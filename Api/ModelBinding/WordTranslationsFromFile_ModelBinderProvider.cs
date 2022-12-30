using FiveWords.DataObjects;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FiveWords.Api.ModelBinding
{
    public class WordTranslationsFromFile_ModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(ICollection<WordTranslation>))
                return new WordTranslationsFromFile_ModelBinder();
            return null;
        }
    }
}
