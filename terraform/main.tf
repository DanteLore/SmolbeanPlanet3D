terraform {
  required_version = ">= 1.3"
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = ">= 5.0"
    }
  }
}

########################################
# This is the terraform to set up file 
# uploads from github actions to
# dantelore.com in AWS
########################################

# Your AWS region
variable "aws_region" {
  type        = string
  description = "AWS region to create IAM resources in."
  default     = "eu-west-1"
}

# GitHub owner (your username or org)
variable "github_owner" {
  type        = string
  description = "GitHub owner (user or org)."
  default     = "DanteLore"
}

# Repository name
variable "github_repo" {
  type        = string
  description = "GitHub repository name."
  default     = "SmolbeanPlanet3D"
}

# Which refs are allowed to assume the role
# Keep 'release' branch and all tags by default
variable "allowed_refs" {
  type        = list(string)
  description = "List of refs allowed to assume the role (e.g., refs/heads/release, refs/tags/*)."
  default     = ["refs/heads/release", "refs/tags/*"]
}

# S3 bucket and prefix to restrict access to
variable "s3_bucket" {
  type        = string
  description = "Target S3 bucket."
  default     = "dantelore.com"
}

variable "s3_prefix" {
  type        = string
  description = "Prefix path inside the bucket."
  default     = "smolbeanplanet"
}

# Name for the IAM role
variable "role_name" {
  type        = string
  description = "Name of the IAM role GitHub Actions will assume."
  default     = "GitHubActions-Smolbeanplanet-Upload"
}

provider "aws" {
  profile = "dantelore"
  region = var.aws_region
}

locals {
  # Build the list of allowed 'sub' claims for the trust policy
  allowed_subs = [for ref in var.allowed_refs : "repo:${var.github_owner}/${var.github_repo}:ref:${ref}"]
}

########################################
# GitHub OIDC provider (one per account)
########################################

# Create the OIDC provider if you don't already have it.
# Safe to run once per account; if you already created it elsewhere, just keep this here.
resource "aws_iam_openid_connect_provider" "github" {
  url = "https://token.actions.githubusercontent.com"

  client_id_list = ["sts.amazonaws.com"]

  # Current GitHub OIDC thumbprints. (AWS allows multiple; keep both common ones.)
  thumbprint_list = [
    "6938fd4d98bab03faadb97b34396831e3780aea1",
    "1c58a3a8518e8759bf075b76b750d4f0f3f1f1c7"
  ]
}

########################################
# Assume-role trust policy (GitHub → IAM)
########################################

data "aws_iam_policy_document" "github_trust" {
  statement {
    effect  = "Allow"
    actions = ["sts:AssumeRoleWithWebIdentity"]

    principals {
      type        = "Federated"
      identifiers = [aws_iam_openid_connect_provider.github.arn]
    }

    condition {
      test     = "StringEquals"
      variable = "token.actions.githubusercontent.com:aud"
      values   = ["sts.amazonaws.com"]
    }

    condition {
      test     = "StringLike"
      variable = "token.actions.githubusercontent.com:sub"
      values   = local.allowed_subs
    }
  }
}

resource "aws_iam_role" "github_actions" {
  name               = var.role_name
  assume_role_policy = data.aws_iam_policy_document.github_trust.json
  description        = "Role assumed by GitHub Actions from ${var.github_owner}/${var.github_repo} to upload to s3://${var.s3_bucket}/${var.s3_prefix}/"
}

########################################
# Least-privilege S3 policy for your path
########################################

data "aws_iam_policy_document" "s3_write_prefix" {
  statement {
    sid     = "ListBucketUnderPrefix"
    effect  = "Allow"
    actions = ["s3:ListBucket"]
    resources = [
      "arn:aws:s3:::${var.s3_bucket}"
    ]

    condition {
      test     = "StringLike"
      variable = "s3:prefix"
      values   = ["${var.s3_prefix}/*"]
    }
  }

  statement {
    sid     = "WriteUnderPrefix"
    effect  = "Allow"
    actions = [
      "s3:PutObject",
      "s3:DeleteObject",
      "s3:PutObjectTagging"
    ]
    resources = [
      "arn:aws:s3:::${var.s3_bucket}/${var.s3_prefix}/*"
    ]
  }
}

resource "aws_iam_policy" "s3_write_prefix" {
  name        = "${var.role_name}-S3WritePrefix"
  description = "Allow writes only to s3://${var.s3_bucket}/${var.s3_prefix}/*"
  policy      = data.aws_iam_policy_document.s3_write_prefix.json
}

resource "aws_iam_role_policy_attachment" "attach_s3" {
  role       = aws_iam_role.github_actions.name
  policy_arn = aws_iam_policy.s3_write_prefix.arn
}

########################################
# Outputs
########################################

output "role_arn" {
  description = "IAM Role ARN to use in GitHub Actions."
  value       = aws_iam_role.github_actions.arn
}

output "bucket_prefix" {
  value = "s3://${var.s3_bucket}/${var.s3_prefix}/"
}
