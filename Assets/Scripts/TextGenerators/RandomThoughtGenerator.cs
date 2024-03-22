using System.Text;
using UnityEngine;

public static class RandomThoughtGenerator
{
    private static readonly string[] things =
    {
        "dodo",
        "tree",
        "wave",
        "rock",
        "cloud",
        "mountain",
        "ship",
        "house",
        "plank",
        "cliff",
        "wreck",
        "sea",
        "ocean",
        "stone"
    };

    private static readonly string[] foods =
    {
        "steak",
        "soup",
        "stew",
        "bread"
    };

    private static readonly string[] verbs =
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

    private static readonly string[] presentParticipals =
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

    private static readonly string[] pastParticipals =
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

    private static readonly string[] sentences =
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
        "Feels like a lifetime since I last ate good [food]",
        "Dreaming of [food]",
        "I think I get less [food] than that [thing]",
        "Glory to you! Your [presentParticipal] is a credit to us all",
        "I wish I was better at [presentParticipal]",
        "The Gods sure have blessed us with [thing]s",
        "Maybe this island isn't as bad as all that... except for the [thing]s",
        "[presentParticipal], [presentParticipal], [presentParticipal] all day long!",
        "I once saw a [presentParticipal] [thing].  Or maybe it was a dream?",
        "The [thing] does not care for material things, it simply is",
        "Do dodos dream of electric [thing]?"
    };

    public static string GetThought()
    {
        StringBuilder result = new(
            Get(sentences)
            .Replace("[thing]", Get(things))
            .Replace("[verb]", Get(verbs))
            .Replace("[presentParticipal]", Get(presentParticipals))
            .Replace("[pastParticipal]", Get(pastParticipals))
            .Replace("[food]", Get(foods))
            );

        result[0] = char.ToUpper(result[0]);
        return result.ToString();
    }

    private static string Get(string[] strs)
    {
        return strs[Random.Range(0, strs.Length)];
    }
}
