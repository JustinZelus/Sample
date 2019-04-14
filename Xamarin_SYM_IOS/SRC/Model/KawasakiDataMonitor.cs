using System;
using System.Collections.Generic;

namespace Model
{
    /// <summary>
    /// 目前用於DM數據顯示用結構，之後最好修改為資料庫之DM儲存結構類別DmData，
    /// 並在DmData中新增RawValues屬性，用以儲存從V.Dialogue收到之原始Bytes數據，
    /// 和新增DisplayValues屬性用於UI顯示用
    /// </summary>
    public class KawasakiDataMonitor
    {
        string mLongName;
        float mValue;
        string unit;
        uint mNumberOfDecimals;
        Dictionary<int, string> mPatternDisplayList;
        string mShortName;
        float? mMinValue = null;
        float? mMaxValue = null;

        public KawasakiDataMonitor(string longName,
                                    float mValue,
                                    string unit,
                                    uint numberOfDecimals,
                                    Dictionary<int, string> patternDisplayList,
                                    String shortName,
                                    float? minValue,
                                    float? maxValue)
        {
            this.mLongName = longName;
            this.mValue = mValue;
            this.unit = unit;
            this.mNumberOfDecimals = numberOfDecimals;
            this.mPatternDisplayList = patternDisplayList;
            this.mShortName = shortName;
            this.mMinValue = minValue;
            this.mMaxValue = maxValue;
        }

        public String LongName
        {
            get
            {
                return mLongName;
            }
        }

        public float Value
        {
            set { mValue = value; }
            get { return mValue; }
        }


        public String Unit
        {
            set { unit = value; }
            get { return unit; }
        }

        public uint NumberOfDecimals
        {
            set { mNumberOfDecimals = value; }
            get { return mNumberOfDecimals; }
        }

        public Dictionary<int, string> PatternDisplayList
        {
            set { mPatternDisplayList = value; }
            get { return mPatternDisplayList; }
        }

        public String ShortName
        {
            set { mShortName = value; }
            get { return mShortName; }
        }

        public float? MinValue
        {
            set { mMinValue = value; }
            get { return mMinValue; }
        }

        public float? MaxValue
        {
            set { mMaxValue = value; }
            get { return mMaxValue; }
        }

    }
}
