﻿using System;
using System.Linq;
using Chotiskazal.Api.Services;
using Chotiskazal.ConsoleTesting.Services;
using Chotiskazal.DAL;
using Chotiskazal.DAL.Services;
using Chotiskazal.LogicR;

namespace Chotiskazal.ApI.Exams
{
    public class ClearAssemblePhraseExam : IExam
    {
        public bool NeedClearScreen => true;
        public string Name => "Assemble phrase";

        public ExamResult Pass(ExamService service, UserWordForLearning word, UserWordForLearning[] examList)
        {
            if (!word.Phrases.Any())
                return ExamResult.Impossible;

            var targetPhrase = word.Phrases.GetRandomItem();

            string shuffled;
            while (true)
            {
                var split = 
                    targetPhrase.EnPhrase.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length < 2)
                    return ExamResult.Impossible;

                shuffled = string.Join(" ", split.Randomize());
                if(shuffled!= targetPhrase.EnPhrase)
                    break;
            }

            Console.WriteLine("Words in phrase are shuffled. Write them in correct order:\r\n'" +  shuffled+ "'");
            string entry= null;
            while (string.IsNullOrWhiteSpace(entry))
            {
                Console.WriteLine(":");
                entry = Console.ReadLine().Trim();
            }

            if (string.CompareOrdinal(targetPhrase.EnPhrase, entry) == 0)
            {
                service.RegistrateSuccessAsync(word);
                return ExamResult.Passed;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Original phrase was: '{targetPhrase.EnPhrase}'");
            Console.ResetColor();
            service.RegistrateFailureAsync(word);
            return ExamResult.Failed;
        }
    }
}