using UnityEngine;

public static class ColonistNameGenerator
{
    private const float nicknameProbability = 25;

    private static readonly string[] firstnames = {
        "Dave",
        "Dante",
        "Astrid",
        "Freya",
        "Garth",
        "Hallie",
        "Kristofer",
        "Jensen",
        "Kirsten",
        "Linnea",
        "Mia",
        "Berit",
        "Gro",
        "Inga",
        "Norna",
        "Olga",
        "Sigrid",
        "Tora",
        "Trina",
        "Vivica",
        "Anders",
        "Booth",
        "Carlson",
        "Garald",
        "Gustav",
        "Jens",
        "Jurgen",
        "Norell",
        "Osman",
        "Waddell",
        "Dag",
        "Dennie",
        "Eino",
        "Kåre",
        "Arne",
        "Loki"
    };

    private static readonly string[] nicknames =
    {
        "Crack Shot",
        "Hunter",
        "Chief",
        "The Navigator",
        "Braveheart",
        "Longbeard",
        "Long Legs",
        "Three Pints",
        "Long Shot",
        "Baby Face",
        "Eagle",
        "Dolph",
        "Iron Hands",
        "Crafty",
        "Thor",
        "Trickster",
        "Dead Eye",
        "The Hammer"
    };

    private static readonly string[] surnames = {
        "Strongintharm",
        "Oarsmann",
        "Steersmann",
        "Dantesson",
        "Thorenson",
        "Davesdottir",
        "Lambert",
        "Edman",
        "Eskelson",
        "Frydenlund",
        "Gustavson",
        "Guttormson",
        "Falk",
        "Dal",
        "Bro",
        "Brand",
        "Hjorth",
        "Hohlt",
        "Krog",
        "Kohlsrud",
        "Ness",
        "Nord",
        "Lind",
        "Karlson",
        "Nybo",
        "Toft",
        "Thorp",
        "Thostenson",
        "Trulson",
        "Westergaard",
        "Steenberg",
        "Salverson"
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
