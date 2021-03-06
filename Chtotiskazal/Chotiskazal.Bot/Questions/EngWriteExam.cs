﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Chotiskazal.ConsoleTesting.Services;
using Chotiskazal.DAL;

namespace Chotiskazal.Bot.Questions
{
    public class EngWriteExam : IExam
    {
        public bool NeedClearScreen => false;

        public string Name => "Eng Write";

        public async Task<ExamResult> Pass(ChatIO chatIo, ExamService service, UserWordForLearning word, UserWordForLearning[] examList)
        {
            var translations = word.GetTranslations();
            var minCount = translations.Min(t => t.Count(c => c == ' '));
            if (minCount>0 && word.PassedScore< minCount*4)
                return ExamResult.Impossible;

            await chatIo.SendMessage($"=====>   {word.EnWord}    <=====\r\nWrite the translation... ");
            var translation = await chatIo.WaitUserTextInput();
            if (string.IsNullOrEmpty(translation))
                return ExamResult.Retry;

            if (translations.Any(t => string.Compare(translation, t, StringComparison.OrdinalIgnoreCase) == 0))
            {
                await service.RegistrateSuccessAsync(word);
                return ExamResult.Passed;
            }
            else
            
            {
                //TODO
                /*
                if (word.GetAllMeanings()
                    .Any(t => string.Compare(translation, t, StringComparison.OrdinalIgnoreCase) == 0))
                {
                    await chat.SendMessage($"Choosen translation is out of scope (but it is correct). Expected translations are: " + word.UserTranslations);
                    return ExamResult.Impossible;
                }
                await chat.SendMessage("The translation was: "+ word.UserTranslations);
                */
               await service.RegistrateFailureAsync(word);
                
                return ExamResult.Failed; 
            }
        }
    }
}