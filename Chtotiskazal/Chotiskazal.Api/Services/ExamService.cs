using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chotiskazal.ApI.Exams;
using Chotiskazal.Dal;
using Chotiskazal.DAL;
using Chotiskazal.Dal.Services;
using Chotiskazal.LogicR;

namespace Chotiskazal.ConsoleTesting.Services
{
    public class ExamService
    {
        private ExamsAndMetricService _examsAndMetricService;
        private DictionaryService _dictionaryService;
        private UsersWordsService _usersWordsService;

        public ExamService( ExamsAndMetricService examsAndMetricService,
            DictionaryService dictionaryService,UsersWordsService usersWordsService)
        {
            _examsAndMetricService = examsAndMetricService;
            _dictionaryService = dictionaryService;
            _usersWordsService = usersWordsService;
        }

     /*   public UserWordForLearning[] GetWordsForLearningOLD(int userId, int count, int maxTranslationSize)
        {
            //получаю все худщие пары юзера
            var fullPairs =_usersPairsService.GetWorstForUser(userId, count);
           
            var wordsForLearning = new List<UserWordForLearning>();
            foreach (var pairModel in fullPairs)
            {
                //получаю саму пару из словаря и его фразы и ее фразы
                var pairFromDictionary = _dictionaryService.GetPairWithPhrasesByIdOrNull(pairModel.PairId);
                //получаю все его переводы(для данного слова!, а не только пару)
                var allTranslates = _dictionaryService.GetAllTranslations(pairFromDictionary.EnWord);
                //получаю все метрики для данной пары
                var metrics = _examsAndMetricService.GetAllMetricsForPair(pairModel.MetricId);
                //получаю все переводы данного слова для юзера
                var allTranslationsOfWordForUser = _usersPairsService.GetAllUserTranslatesForWord(userId, pairFromDictionary.EnWord);
                
                // составляю их этого Ford For learning
                var wordForLearning = new WordForLearning(pairModel,pairFromDictionary,metrics,allTranslates,allTranslationsOfWordForUser);

                // отбор, если меньше трех переводов, то не отбираем, а берем все
                var translations = wordForLearning.GetTranslations().ToArray();
                if (translations.Length <= maxTranslationSize)
                {
                    wordsForLearning.Add(wordForLearning);
                    continue;
                }

                // ели больше трех переводов, то отбираем нужные
                var usedTranslations = translations.Randomize().Take(maxTranslationSize).ToArray();
                // назначаем их слову. Тут по задумке могут оказаться и фразы.
                wordForLearning.SetTranslations(usedTranslations);
             
                // убирает из фраз те фразы, которые оказались в переводах. Потмоу что мы их добавили в словарь пар
               //TODO это зачем это? по идее мы убрали фразы, когда их переносили. Тут не может быть этих фраз
               //вернуться позже и посмотреть на необходимость этого 
               /*  for (int i = 0; i < wordForLearning.Phrases.Count; i++)
                {
                    var phrase = wordForLearning.Phrases[i];
                    if (!usedTranslations.Contains(phrase.Translation))
                        wordForLearning.Phrases.RemoveAt(i);
                }//
               
               wordsForLearning.Add(wordForLearning);
            }
            return wordsForLearning.ToArray();
        }*/

     public UserWordForLearning[] GetWordsForLearning(int userId, int count, int maxTranslationSize)
     {
         //получаю все худщие пары юзера
         var wordsForLearning = _usersWordsService.GetWorstForUser(userId, count);

         foreach (var wordForLearning in wordsForLearning)
         {
             // отбор, если меньше трех переводов, то не отбираем, а берем все
             var translations = wordForLearning.GetTranslations().ToArray();
             if (translations.Length <= maxTranslationSize)
                 continue;

             // ели больше трех переводов, то отбираем нужные и присваиваем нашему слову для изучения
             var usedTranslations = translations.Randomize().Take(maxTranslationSize).ToArray();
             wordForLearning.SetTranslation(usedTranslations);


             // убирает из фраз те фразы, которые оказались в переводах. Потмоу что мы их добавили в словарь пар
             //TODO это зачем это? по идее мы убрали фразы, когда их переносили. Тут не может быть этих фраз
             //вернуться позже и посмотреть на необходимость этого 

         }

         return wordsForLearning.ToArray();
     }

     //TODO Зачем этот метод и метод выше, а еще GetTestWord
        public UserWordForLearning[] GetPairsForTestWords(int userId, int delta, int randomRate)
        {
              return _usersWordsService.GetWorstTestWordForUser(userId, delta,randomRate);
        }

        public new List<UserWordForLearning> PreparingExamsList(UserWordForLearning[] learningWords)
        {
            var examsList = new List<UserWordForLearning>(learningWords.Length * 4);
            //Every learning word appears in test from 2 to 4 times

            examsList.AddRange(learningWords.Randomize());
            examsList.AddRange(learningWords.Randomize());
            examsList.AddRange(learningWords.Randomize().Where(w => RandomTools.Rnd.Next() % 2 == 0));
            examsList.AddRange(learningWords.Randomize().Where(w => RandomTools.Rnd.Next() % 2 == 0));

            while (examsList.Count > 32)
            {
                examsList.RemoveAt(examsList.Count - 1);
            }
            return examsList;
        }
        public UserWordForLearning[] GetTestWords(int userId,List<UserWordForLearning> examsList)
        {
            //TODO изучть по какому принципу получаем RandomRATE. связан ли он с прогрессом подбираемых слов.
            //TODO Или тут вообще рандомные слова будут
            var delta = Math.Min(7, (32 - examsList.Count));
            UserWordForLearning[] testWords = new UserWordForLearning[0];
            if (delta > 0)
            {
                var randomRate = 8 + RandomTools.Rnd.Next(5);
                return testWords = GetPairsForTestWords(userId, delta, randomRate);
            }
            return testWords;
        }

        public void UpdateAgingAndRandomize(int i) => _usersWordsService.UpdateAgingAndRandomize(i);

        //TODO разобраться с QuestionMetric
        public void SaveQuestionMetrics(QuestionMetric questionMetric) =>
            _examsAndMetricService.SaveQuestionMetrics(questionMetric);
        
        public void RegistrateExam(int userId, DateTime started, int examsCount, int examsPassed) =>
            _examsAndMetricService.RegistrateExam(userId, started, examsCount, examsPassed);

        public void RegistrateSuccess(UserWordForLearning userWordForLearning) => 
            _usersWordsService.RegistrateSuccess(userWordForLearning);
       
        public void RegistrateFailure(UserWordForLearning userWordForLearning) =>
            _usersWordsService.RegistrateFailure(userWordForLearning);

      
        
        //TODO additional methods
        //use for Graph Mode
        public UserWordForLearning[] GetAllExamedWords(in int userId)
        {
            throw new NotImplementedException();
        }
        //use for Graph mode
        public Exam[] GetAllExams()
        {
            throw new NotImplementedException();
        }

        // TODO use for RuWriteExam(now i don't use it) need to understand
      /*public WordForLearning Get(string word)
      {
          var pairFromDb = _usersWordService.;

        
          public PairModel GetOrNull(string word)
          {
              if (!File.Exists(DbFile))
                  return null;
              using (var cnn = SimpleDbConnection())
              {
                  cnn.Open();
                  var result = cnn.Query<PairModel>(
                      @"SELECT * FROM Words WHERE OriginWord = @word", new { word }).FirstOrDefault();
                  return result;
              }
          }
      }*/




       


       
    }
}
