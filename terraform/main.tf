########################################
# This is the terraform to set up file 
# uploads from github actions to
# dantelore.com in AWS
########################################

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
# Variables
########################################

variable "aws_region" {
  type        = string
  default     = "eu-west-1"
}

variable "aws_profile" {
  type        = string
  default     = "dantelore"   # your local AWS CLI profile
}

variable "github_owner" {
  type        = string
  default     = "DanteLore"
}

variable "github_repo" {
  type        = string
  default     = "SmolbeanPlanet3D"
}

# Allowed refs that may assume the role
variable "allowed_refs" {
  type        = list(string)
  default     = ["refs/heads/release", "refs/tags/*"]
}

# S3 target
variable "s3_bucket" {
  type        = string
  default     = "dantelore.com"
}

variable "s3_prefix" {
  type        = string
  default     = "smolbeanplanet"
}

# IAM role name
variable "role_name" {
  type        = string
  default     = "GitHubActions-Smolbeanplanet-Upload"
}

provider "aws" {
  profile = var.aws_profile
  region  = var.aws_region
}

data "aws_caller_identity" "current" {}

locals {
  allowed_subs = [
    for ref in var.allowed_refs :
    "repo:${var.github_owner}/${var.github_repo}:ref:${ref}"
  ]
}

########################################
# GitHub OIDC provider
########################################

resource "aws_iam_openid_connect_provider" "github" {
  url = "https://token.actions.githubusercontent.com"

  client_id_list = ["sts.amazonaws.com"]

  # GitHub OIDC thumbprints (as of 2025)
  thumbprint_list = [
    "6938fd4d98bab03faadb97b34396831e3780aea1",
    "1c58a3a8518e8759bf075b76b750d4f0f3f1f1c7"
  ]
}

########################################
# Trust policy (GitHub → IAM)
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
  description        = "Role for GitHub Actions (${var.github_owner}/${var.github_repo}) to upload to s3://${var.s3_bucket}/${var.s3_prefix}/ and invalidate CloudFront."
}

########################################
# S3 least-privilege policy (prefix-only)
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
# CloudFront invalidation permission (all distributions)
########################################

data "aws_iam_policy_document" "cloudfront_invalidate" {
  statement {
    sid     = "CreateInvalidation"
    effect  = "Allow"
    actions = ["cloudfront:CreateInvalidation"]
    resources = ["*"]   # allow invalidations on any distribution
  }
}

resource "aws_iam_policy" "cloudfront_invalidate" {
  name        = "${var.role_name}-CloudFrontInvalidate"
  description = "Allow CloudFront invalidations on any distribution"
  policy      = data.aws_iam_policy_document.cloudfront_invalidate.json
}

resource "aws_iam_role_policy_attachment" "attach_cf" {
  role       = aws_iam_role.github_actions.name
  policy_arn = aws_iam_policy.cloudfront_invalidate.arn
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
