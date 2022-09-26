﻿using CsvHelper.Configuration;
using FiveWords.DataObjects;
using FiveWords.Repository.Interfaces;

namespace FiveWords.Repository.CsvRepository;

internal class WordsCsvRepository : OneFileCsvRepository<Word, int, ClassMap<Word>>, IWordsRepository
{
    protected internal WordsCsvRepository(string homeDirectoryPath, string fileName) : base(homeDirectoryPath, fileName) { }

    public IEnumerable<Word> GetByWritingFilter(Predicate<string?> writingFilter) => GetAll()
        .Where(kvp => writingFilter(kvp.Value.Writing))
        .Select(kvp => kvp.Value);

    protected override ClassMap<Word> Mapping => new WordMapping();

    private class WordMapping : ClassMap<Word>
    {
        public WordMapping()
        {
            Map(w => w.Id).Name("Id").Index(0);
            Map(w => w.Writing).Name("Writing").Index(1);
            Map(w => w.Language).Name("Language").Index(2);
            //Map(w => w.DefaultTranslate.Writing).Name("DefaultTranslate").Index(3);
        }
    }
}

internal class WordsWithEnglishTranslationId_CsvRepository : OneFileCsvRepository<WordWithEnglishTranslationId, int, ClassMap<WordWithEnglishTranslationId>>, IWordsWithEnglishTranslationRepository
{
    protected internal WordsWithEnglishTranslationId_CsvRepository(string homeDirectoryPath, string fileName) : base(homeDirectoryPath, fileName) { }

    public IEnumerable<WordWithEnglishTranslationId> GetWordsHavingEnglishTranslationId()
    {
        return GetAll().Where(kvp => kvp.Value.DefaultEnglishTranslationId != null).Select(kvp => kvp.Value);
    }

    protected override ClassMap<WordWithEnglishTranslationId> Mapping => new WordWithEnglishTranslationMapping();

    private class WordWithEnglishTranslationMapping : ClassMap<WordWithEnglishTranslationId>
    {
        public WordWithEnglishTranslationMapping()
        {
            Map(w => w.Id).Name("Id").Index(0);
            Map(w => w.Writing).Name("Writing").Index(1);
            Map(w => w.Language).Name("Language").Index(2);
            Map(w => w.DefaultEnglishTranslationId).Name("DefaultEnglishTranslateId").Index(3);
        }
    }
}