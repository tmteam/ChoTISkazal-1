﻿using System;
using MongoDB.Bson;

namespace SayWa.MongoDAL.Dictionary
{
    public class DictionaryTranslation {
        public ObjectId Id { get; set; }
        public string   Word    { get; set; }
        public string   Transcription { get; set; }
        public Language Language    { get; set; }
        public DictionaryExample[] Examples  { get; set; }
    }
}