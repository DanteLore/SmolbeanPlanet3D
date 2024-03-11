using UnityEngine;

public static class DodoNameGenerator
{
    private const float nicknameProbability = 25;

    private static readonly string[] firstnames = {
        "Cluckles",
        "Hootie",
        "Dave",
        "Doreen"
    };

    private static readonly string[] nicknames =
    {
        "Big Beak",
        "Captain",
        "Feathers",
        "Claws"
    };

    private static readonly string[] surnames = {
        "McDodo",
        "Honkington",
        "L'Œuf"
    };

    public static string Generate()
    {
        string firstname = firstnames[Random.Range(0, firstnames.Length)];
        string surname = surnames[Random.Range(0, surnames.Length)];
        string nickname = nicknames[Random.Range(0, nicknames.Length)];

        if (Random.Range(0, 100) < nicknameProbability)
            return $"{firstname} \"{nickname}\" {surname}";
        else
            return $"{firstname} {surname}";
    }
}
