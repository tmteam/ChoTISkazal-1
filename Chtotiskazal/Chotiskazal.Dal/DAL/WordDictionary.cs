﻿using Chotiskazal.LogicR.yapi;
using System.Collections.Generic;
using Chotiskazal.Dal.Enums;

namespace Chotiskazal.DAL
{
    public class WordDictionary
    {

        public WordDictionary() { }

        public WordDictionary(string enWord, string translation, string transcription, TranslationSource sourse)
        {
            EnWord = enWord;
            Transcription = transcription;
            RuWord = translation;
            Sourse = sourse;
        }
        public WordDictionary(string enWord, string translation, string transcription, TranslationSource sourse, List<Phrase> phrases)
        {
            EnWord = enWord;
            Transcription = transcription;
            RuWord = translation;
            Phrases = phrases;
            Sourse = sourse;
        }
        public int PairId { get; set; }
        public string EnWord { get; set; }

        //for one Word has one Translation
        //in table WordsWithTranslation we have same words with different ID and different Translate
        // or we can use composite key(EnWord+Translate).      
        public string RuWord { get; set; }
        public string Transcription { get; set; }

        /* ALTERNATIVE 
               //if IsPhrase is true WordSourse=Id of Word in this table
               //if IsPhrase is false WordSourse=null
               public bool IsPhrase { get; set; }
               public int WordSourse { get;set; }
        */

        public List<Phrase> Phrases { get; set; } = new List<Phrase>();
        public TranslationSource Sourse { get; set; }

    }
    
    
}
