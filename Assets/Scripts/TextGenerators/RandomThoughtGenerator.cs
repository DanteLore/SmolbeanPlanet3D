using UnityEngine;

public static class RandomThoughtGenerator
{
    private static string[] things =
    {
        "dodo",
        "tree",
        "wave",
        "rock",
        "cloud",
        "colonist",
        "mountain",
        "ship",
        "house",
        "plank",
        "cliff",
        "wreck",
        "sea",
        "ocean",
        "stone",
        "blade of grass",
        "dodo feather",
        "steak"
    };

    private static string[] verbs =
    {
        "swim",
        "run",
        "day dream",
        "fight",
        "work",
        "crawl",
        "sleep",
        "dream",
        "talk",
        "sing"
    };

    private static string[] presentParticipals =
    {
        "swimming",
        "running",
        "day dreaming",
        "fighting",
        "working",
        "crawling",
        "sleeping",
        "dreaming",
        "talking",
        "singing"
    };

    private static string[] pastParticipals =
    {
        "swam",
        "ran",
        "day dreamed",
        "fought",
        "worked",
        "crawled",
        "slept",
        "dreamt",
        "talked",
        "sang"
    };

    private static string[] sentences =
    {
        "I wonder if [thing]s can [verb]?",
        "Hanging around can get boring at times",
        "Just relaxing, might go for a [verb] later",
        "Who can [verb] best? Only one way to find out...",
        "I had a dream last night about a [presentParticipal] [thing]",
        "Maybe there'll be a job for me soon",
        "Do di do di doooo",
        "So sick of [thing]s!",
        "This island has so many [thing]s!",
        "I wonder how far I could throw a [thing]",
        "They will write songs about my [presentParticipal]!",
        "Thank the Gods for [presentParticipal] [thing]s",
        "A [thing] in the hand is worth two in the bush",
        "Every [thing] has an epic saga of its own",
        "I [pastParticipal] for hours yeterday",
        "I can't remember when I last [pastParticipal]",
        "Which [thing] [pastParticipal] best?"
    };

    public static string GetThought()
    {
        return Get(sentences)
            .Replace("[thing]", Get(things))
            .Replace("[verb]", Get(verbs))
            .Replace("[presentParticipal]", Get(presentParticipals))
            .Replace("[pastParticipal]", Get(pastParticipals));
    }

    private static string Get(string[] strs)
    {
        return strs[Random.Range(0, strs.Length)];
    }
}
