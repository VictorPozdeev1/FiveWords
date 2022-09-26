namespace FiveWords.ToBeDeleted
{
    [ToBeDeleted]
    public class ToBeDeletedAttribute : Attribute
    {
    }

    //[ToBeDeleted]
    //public class TempStupidRepository : IWordsRepository
    //{
    //    private TempStupidRepository() { }
    //    //private static TempStupidRepository? instance;
    //    //public static TempStupidRepository Instance
    //    //{
    //    //    get
    //    //    {
    //    //        if (instance == null)
    //    //        {
    //    //            instance = new TempStupidRepository();
    //    //            instance.CreateDefaultWordList();
    //    //        }
    //    //        return instance;
    //    //    }
    //    //}

    //    readonly List<Word> allWords = new();

    //    public IQueryable<Word> GetAll()
    //    {
    //        return allWords.AsQueryable();
    //    }

    //    int id = 0;
    //    //[ToBeDeleted]
    //    //void AddWordPair(string russianWordWriting, List<Word> russianWords, string englishWordWriting, List<Word> englishWords)
    //    //{
    //    //    id++;
    //    //    Word russianWord = new Word(russianWordWriting, Language.Russian);
    //    //    russianWord.Id = id;

    //    //    Word englishWord = new Word(englishWordWriting, Language.English);
    //    //    englishWord.Id = id;

    //    //    russianWord.DefaultTranslate = englishWord;
    //    //    russianWords.Add(russianWord);
    //    //    englishWords.Add(englishWord);
    //    //}

    //    //[ToBeDeleted]
    //    //void CreateDefaultWordList()
    //    //{
    //    //    AddWordPair("рюкзак", allWords, "a backpack", allWords);
    //    //    AddWordPair("сумка", allWords, "a bag", allWords);
    //    //    AddWordPair("открытка", allWords, "a card", allWords);
    //    //    AddWordPair("задержка", allWords, "a delay", allWords);
    //    //    AddWordPair("пункт назначения", allWords, "a destination", allWords);
    //    //    AddWordPair("карточка (банковская)", allWords, "a card", allWords);
    //    //    AddWordPair("багаж", allWords, "luggage", allWords);
    //    //    AddWordPair("карта", allWords, "a map", allWords);
    //    //    AddWordPair("чемодан", allWords, "a suitcase", allWords);
    //    //    AddWordPair("билет", allWords, "a ticket", allWords);
    //    //    AddWordPair("велосипед", allWords, "a bicycle", allWords);
    //    //    AddWordPair("автобус", allWords, "a bus", allWords);
    //    //    AddWordPair("автомобиль", allWords, "a car", allWords);
    //    //    AddWordPair("корабль", allWords, "a ship", allWords);
    //    //    AddWordPair("такси", allWords, "a cab", allWords);
    //    //    AddWordPair("трамвай", allWords, "a trolley", allWords);
    //    //    AddWordPair("страховка", allWords, "insurance", allWords);
    //    //    AddWordPair("поезд", allWords, "a train", allWords);
    //    //    AddWordPair("самолёт", allWords, "a plane", allWords);
    //    //    WriteToCsv(allWords.Where(w => w.Language == Language.Russian).ToList(), @"russian-words.scv");
    //    //    WriteToCsv(allWords.Where(w => w.Language == Language.English).ToList(), @"english-words.scv");
    //    //}

    //    public Word Get(int id)
    //    {
    //        // В этом репозитории id вообще нет !!
    //        throw new InvalidOperationException();
    //    }

    //public void WriteToCsv(List<Word> toWrite, string filePath)
    //{
    //    using (var writer = new StreamWriter(filePath))
    //    using (var csvWriter = new CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture))
    //    {
    //        csvWriter.Context.RegisterClassMap<WordMapping>();
    //        csvWriter.WriteRecords(toWrite);
    //    }
    //}

    //private class WordMapping : ClassMap<Word>
    //{
    //    public WordMapping()
    //    {
    //        Map(w => w.Id).Name("Id").Index(0);
    //        Map(w => w.Writing).Name("Writing").Index(1);
    //        Map(w => w.Language).Name("Language").Index(2);
    //        Map(w => w.DefaultTranslate.Id).Name("DefaultTranslate").Index(3);
    //    }
    //}
    //}
}