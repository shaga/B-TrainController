using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTrainDemoApp.Models;

namespace BTrainDemoApp.Data
{
    class OsakaLoopLineStations : ReadOnlyDictionary<string, LoopStationInfo>
    {
        public const string DefaultKey = "Tennoji";

        private static IDictionary<string, LoopStationInfo> _stations = new Dictionary<string, LoopStationInfo>()
        {
            {"Tennoji", new LoopStationInfo(3, 12, "天王寺", "新今宮・西九条", "鶴橋・京橋", "Shin-Imamiya", "Teradacho", false) },
            {"Teradacho", new LoopStationInfo(3, 14, "寺田町", "天王寺・新今宮", "鶴橋・京橋", "Tennoji", "Momodani", false) },
            {"Momodani", new LoopStationInfo(3, 16, "桃　谷", "天王寺・新今宮", "鶴橋・京橋", "Teradacho", "Tsuruhashi", false) },
            {"Tsuruhashi", new LoopStationInfo(3, 18, "鶴　橋", "天王寺・新今宮", "京橋・大阪", "Momodani", "Tamatsukuri", false) },
            {"Tamatsukuri", new LoopStationInfo(3, 20, "玉　造", "鶴橋・天王寺", "京橋・大阪", "Tsuruhashi", "Morinomiya", false) },
            {"Morinomiya", new LoopStationInfo(1, 19, "森ノ宮", "鶴橋・天王寺", "京橋・大阪", "Tamatsukuri", "Osakajokoen") },
            {"Osakajokoen", new LoopStationInfo(1, 17, "大阪城公園", "鶴橋・天王寺", "京橋・大阪", "Morinomiya", "Kyobashi") },
            {"Kyobashi", new LoopStationInfo(1, 15, "京　橋", "鶴橋・天王寺", "大阪・西九条", "Osakajokoen", "Sakuranomiya") },
            {"Sakuranomiya", new LoopStationInfo(1, 13, "桜ノ宮", "京橋・鶴橋", "大阪・西九条", "Kyobashi", "Temma") },
            {"Temma", new LoopStationInfo(1, 11, "天　満", "京橋・鶴橋", "大阪・西九条", "Sakuranomiya", "Osaka") },
            {"Osaka", new LoopStationInfo(1, 9, "大　阪", "京橋・鶴橋", "西九条・新今宮", "Temma", "Fukushima") },
            {"Fukushima", new LoopStationInfo(1, 7, "福　島", "大阪・京橋", "西九条・新今宮", "Osaka", "Noda") },
            {"Noda", new LoopStationInfo(1, 5, "野　田", "大阪・京橋", "西九条・新今宮", "Fukushima", "Nishikujo") },
            {"Nishikujo", new LoopStationInfo(1, 3, "西九条", "大阪・京橋", "新今宮・天王寺", "Noda", "Bentencho") },
            {"Bentencho", new LoopStationInfo(3, 2, "弁天町", "西九条・大阪", "新今宮・天王寺", "Nishikujo", "Taisho", false) },
            {"Taisho", new LoopStationInfo(3, 4, "大　正", "西九条・大阪", "新今宮・天王寺", "Bentencho", "Ashiharabashi", false) },
            {"Ashiharabashi", new LoopStationInfo(3, 6, "芦原橋", "西九条・大阪", "新今宮・天王寺", "Taisho", "Imamiya", false) },
            {"Imamiya", new LoopStationInfo(3, 8, "今　宮", "西九条・大阪", "新今宮・天王寺", "Ashiharabashi", "Shin-Imamiya", false) },
            {"Shin-Imamiya", new LoopStationInfo(3, 10, "新今宮", "西九条・大阪", "天王寺・鶴橋", "Imamiya", "Tennoji", false) }
        };

        //private static IDictionary<string, StationInfo> _stations = new Dictionary<string, StationInfo>()
        //{
        //    {"Osaka", new StationInfo("大阪", "おおさか", "Osaka", "Temma", "Fukushima")},
        //    {"Temma", new StationInfo("天満", "てんま", "Temma", "Sakuranomiya", "Osaka")},
        //    {"Sakuranomiya", new StationInfo("桜ノ宮", "さくらのみや", "Sakuranomiya", "Kyobashi", "Temma")},
        //    {"Kyobashi", new StationInfo("京橋", "きょうばし", "Kyobashi", "Osakajokoen", "Sakuranomiya")},
        //    {"Osakajokoen", new StationInfo("大阪城公園", "おおさかじょうこうえん", "Osakajokoen", "Morinomiya", "Kyobashi")},
        //    {"Morinomiya", new StationInfo("森ノ宮", "もりのみや", "Morinomiya", "Tamatsukuri", "Osakajokoen")},
        //    {"Tamatsukuri", new StationInfo("玉造", "たまつくり", "Tamatsukuri", "Tsuruhashi", "Tamatsukuri")},
        //    {"Tsuruhashi", new StationInfo("鶴橋", "つるはし", "Tsuruhashi", "Momodani", "Tamatsukuri")},
        //    {"Momodani", new StationInfo("桃谷", "ももだに", "Momodani", "Teradacho", "Tsuruhashi")},
        //    {"Teradacho", new StationInfo("寺田町", "てらだちょう", "Teradacho", "Tennoji", "Momodani")},
        //    {"Tennoji", new StationInfo("天王寺", "てんのうじ", "Tennoji", "Shin-Imamiya", "Teradacho")},
        //    {"Shin-Imamiya", new StationInfo("新今宮", "しんいまみや", "Shin-Imamiya", "Imamiya", "Tennoji")},
        //    {"Imamiya", new StationInfo("今宮", "いまみや", "Imamiya", "Ashiharabashi", "Shin-Imamiya")},
        //    {"Ashiharabashi", new StationInfo("芦原橋", "あしはらばし", "Ashiharabashi", "Taisho", "Imamiya")},
        //    {"Taisho", new StationInfo("大正", "たいしょう", "Taisho", "Bentencho", "Ashiharabashi") },
        //    {"Bentencho", new StationInfo("弁天町", "べんてんちょう", "Bentencho", "Nishikujo", "Taisho") },
        //    {"Nishikujo", new StationInfo("西九条", "にしくじょう", "Nishikujo", "Noda", "Bentencho") },
        //    {"Noda", new StationInfo("野田", "のだ", "Noda", "Fukushima", "Nishikujo") },
        //    {"Fukushima", new StationInfo("福島", "ふくしま", "Fukushima", "Osaka", "Noda")  }
        //};

        public OsakaLoopLineStations() : base(_stations)
        {
        }

        public LoopStationInfo GetNextOutStationInfo(string key)
        {
            if (!ContainsKey(key)) return null;

            var nextKey = this[key].NextOuter;

            return ContainsKey(nextKey) ? this[nextKey] : null;
        }

        public LoopStationInfo GetNextInStationInfo(string key)
        {
            if (!ContainsKey(key)) return null;

            var nextKey = this[key].NextInner;

            return ContainsKey(nextKey) ? this[nextKey] : null;
        }
    }
}
