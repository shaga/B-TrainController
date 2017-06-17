using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTrainDemoApp.Models;

namespace BTrainDemoApp.Data
{
    class OsakaLoopLineStations : ReadOnlyDictionary<string, StationInfo>
    {
        public const string DefaultKey = "Osaka";

        private static IDictionary<string, StationInfo> _stations = new Dictionary<string, StationInfo>()
        {
            {"Osaka", new StationInfo("大阪", "おおさか", "Osaka", "Temma", "Fukushima")},
            {"Temma", new StationInfo("天満", "てんま", "Temma", "Sakuranomiya", "Osaka")},
            {"Sakuranomiya", new StationInfo("桜ノ宮", "さくらのみや", "Sakuranomiya", "Kyobashi", "Temma")},
            {"Kyobashi", new StationInfo("京橋", "きょうばし", "Kyobashi", "Osakajokoen", "Sakuranomiya")},
            {"Osakajokoen", new StationInfo("大阪城公園", "おおさかじょうこうえん", "Osakajokoen", "Morinomiya", "Kyobashi")},
            {"Morinomiya", new StationInfo("森ノ宮", "もりのみや", "Morinomiya", "Tamatsukuri", "Osakajokoen")},
            {"Tamatsukuri", new StationInfo("玉造", "たまつくり", "Tamatsukuri", "Tsuruhashi", "Tamatsukuri")},
            {"Tsuruhashi", new StationInfo("鶴橋", "つるはし", "Tsuruhashi", "Momodani", "Tamatsukuri")},
            {"Momodani", new StationInfo("桃谷", "ももだに", "Momodani", "Teradacho", "Tsuruhashi")},
            {"Teradacho", new StationInfo("寺田町", "てらだちょう", "Teradacho", "Tennoji", "Momodani")},
            {"Tennoji", new StationInfo("天王寺", "てんのうじ", "Tennoji", "Shin-Imamiya", "Teradacho")},
            {"Shin-Imamiya", new StationInfo("新今宮", "しんいまみや", "Shin-Imamiya", "Imamiya", "Tennoji")},
            {"Imamiya", new StationInfo("今宮", "いまみや", "Imamiya", "Ashiharabashi", "Shin-Imamiya")},
            {"Ashiharabashi", new StationInfo("芦原橋", "あしはらばし", "Ashiharabashi", "Taisho", "Imamiya")},
            {"Taisho", new StationInfo("大正", "たいしょう", "Taisho", "Bentencho", "Ashiharabashi") },
            {"Bentencho", new StationInfo("弁天町", "べんてんちょう", "Bentencho", "Nishikujo", "Taisho") },
            {"Nishikujo", new StationInfo("西九条", "にしくじょう", "Nishikujo", "Noda", "Bentencho") },
            {"Noda", new StationInfo("野田", "のだ", "Noda", "Fukushima", "Nishikujo") },
            {"Fukushima", new StationInfo("福島", "ふくしま", "Fukushima", "Osaka", "Noda")  }
        };

        public OsakaLoopLineStations() : base(_stations)
        {
        }

        public StationInfo GetNextOutStationInfo(string key)
        {
            if (!ContainsKey(key)) return null;

            var nextKey = this[key].NextOut;

            return ContainsKey(nextKey) ? this[nextKey] : null;
        }

        public StationInfo GetNextInStationInfo(string key)
        {
            if (!ContainsKey(key)) return null;

            var nextKey = this[key].NextIn;

            return ContainsKey(nextKey) ? this[nextKey] : null;
        }
    }
}
