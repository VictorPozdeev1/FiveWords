using CsvHelper;
using CsvHelper.Configuration;
using FiveWords.CommonModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace FiveWords.Api.ModelBinding
{
    public class WordTranslationsFromFile_ModelBinder : IModelBinder
    {
        static readonly WordTranslationMapping mapping = new();
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            //var file = bindingContext.ValueProvider.GetValue("uploadingFile");
            var uploadingFile = bindingContext.HttpContext.Request.Form?.Files?[0];
            if (uploadingFile is null)
            {
                bindingContext.ModelState.AddModelError(string.Empty, "Отсутствует файл, из которого предполагалось прочитать данные.");
                return Task.CompletedTask;
            }

            try
            {
                using var stream = uploadingFile.OpenReadStream();
                using var reader = new StreamReader(stream);
                using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
                csvReader.Context.RegisterClassMap(mapping);
                var wordsToAdd = csvReader.GetRecords<WordTranslation>().ToList();
                bindingContext.Result = ModelBindingResult.Success(wordsToAdd);
                return Task.CompletedTask;
            }
            catch /*(Exception exc)*/
            {
                bindingContext.ModelState.AddModelError(string.Empty, "Не удалось прочитать данные из файла." /*exc.Message*/);
                //bindingContext.Result = ModelBindingResult.Failed();
                //return Task.FromException(exc);
                return Task.CompletedTask;
            }
        }

        private class WordTranslationMapping : ClassMap<WordTranslation>
        {
            public WordTranslationMapping()
            {
                Map(w => w.Id).Name("Word").Index(0);
                Map(w => w.Translation).Name("Translation").Index(1);
            }
        }
    }
}
