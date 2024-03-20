public static class SaveGameNameGenerator
{
    private static readonly string[] sillyNouns = {
        "Blubber-nugget",
        "Snollygoster",
        "Skedaddle",
        "Flummoxed",
        "Wibble",
        "Kerfuffle",
        "Flibbertigibbet",
        "Brouhaha",
        "Hullabaloo",
        "Malarkey",
        "Lollygag",
        "Skedoodle",
        "Fiddle-faddle",
        "Rigmarole",
        "Hocus-pocus",
        "Flapdoodle",
        "Dillydally",
        "Ballyhoo",
        "Fuddy-duddy",
        "Hoopla",
        "Hootenanny",
        "Flimflam",
        "Balderdash",
        "Fiddlesticks",
        "Gobbledygook",
        "Codswallop",
        "Potato",
        "po-tay-toes",
        "Hobbit bottom",
        "Ogres",
        "Wangdoodle"
    };

    private static readonly string[] sillyAdjectives = {
        "Amusing",
        "Comical",
        "Hilarious",
        "Witty",
        "Humorous",
        "Entertaining",
        "Lighthearted",
        "Jovial",
        "Waggish",
        "Droll",
        "Chucklesome",
        "Laughable",
        "Mirthful",
        "Playful",
        "Jocular",
        "Whimsical",
        "Silly",
        "Zany",
        "Goofy",
        "Higgledy-piggledy",
        "Eccentric",
        "Awesomesauce",
        "Diddly widdly",
        "Furious",
        "Confused",
        "Puzzling",
        "Buff"
    };

    public static string Generate()
    {
        string noun = sillyNouns[UnityEngine.Random.Range(0, sillyNouns.Length)].ToLower();
        string adjective = sillyAdjectives[UnityEngine.Random.Range(0, sillyAdjectives.Length)];
        int number = UnityEngine.Random.Range(10, 100);

        return $"{adjective} {noun} {number}";
    }
}
