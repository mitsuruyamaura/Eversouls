using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnumTest : MonoBehaviour {

    public enum TIME_TYPE {
        MORNING,
        AFTERNOON,
        EVENING,
        NIGHT,
    }

    public enum DAY_TYPE {
        SUNDAY,
        MANDAY,
        TUESDAY,
        WEDNESDAY,
        THURSDAY,
        FRIDAY,
        SATURDAY
    }

    public enum SEASON_TYPE {
        SPRING,
        SUMMER,
        AUTUMN,
        WINTER
    }

    public DAY_TYPE dayType;
    public TIME_TYPE[] timeTypeArray;
    public List<SEASON_TYPE> seasonTypeList = new List<SEASON_TYPE>();

    void Start() {
        // 文字列を引数で渡した種類のEnumに変換
        string day = "friday";
        dayType = ConvertEnumType.GetEnumTypeFromString<DAY_TYPE>(day);

        // 配列の場合(Select(x => (x))の引数が省略できるので非常に楽)
        string times = "NIGHT,afternoon,Morning";
        //string[] strArray = timeArray.Split(',').ToArray();
        //timeTypeArray = new TIME_TYPE[strArray.Length];
        //for (int i = 0; i < strArray.Length; i++) {
        //    timeTypeArray[i] = GetEnumTypeFromString<TIME_TYPE>(strArray[i]);
        //}
        timeTypeArray = times.Split(',').Select(ConvertEnumType.GetEnumTypeFromString<TIME_TYPE>).ToArray();

        // Listの場合
        string seasons = "summer,Winter,spring,AUTUMN";
        seasonTypeList = seasons.Split(',').Select(ConvertEnumType.GetEnumTypeFromString<SEASON_TYPE>).ToList();
    }

    /// <summary>
    /// stringとEnumのタイプをもらい、文字列をそのタイプのEnumにする
    /// </summary>
    //public IEnum GetEnumTypeFromString<IEnum>(string str) where IEnum : struct {
    //    return (IEnum)Enum.Parse(typeof(IEnum), str, true);
    //}


    // 自分の質問サイト
    // https://teratail.com/questions/247138

    // こちらの書式でOK。テラテイルの回答はこっち。
    //public T GetEnumTypeFromString<T>(string str) where T : struct {
    //    return (T)Enum.Parse(typeof(T), str, true);
    //}

    // テラテイルで回答をもらったが、やはりTにはEnumを指定する必要がある（そこまでは省略できない）
    // 下記の呼び出し用メソッド
    //dayType = GetEnumTypeFromString(dayType, day);

    // メソッドを作らない場合
    //Enum.TryParse<DAY_TYPE>(day, out var dayTime);

    // EnumがあるかBoolで確認できるが、outが必要(引数でもらう必要あり)
    //Enum.IsDefiend(typeof(T), outObj);

    //Enum.TryParseの説明
    //https://qiita.com/masaru/items/a44dc30bfc18aac95015

    //public T GetEnumTypeFromString<T>(T t, string str) where T : struct {
    //    // Enumの種類の確認用
    //    Type tempType = t.GetType();
    //    Debug.Log(tempType);

    //    Type type = typeof(T);
    //    if (!type.IsEnum) {
    //        Debug.Log("Not Enum");
    //    } else  {
    //        if (type == typeof(TIME_TYPE)) {
    //            TIME_TYPE temp = (TIME_TYPE)Enum.Parse(typeof(TIME_TYPE), str, true);
    //            // 一度object型にしないとTが通らない
    //            return (T)(object)temp;
    //        }
    //        if (type == typeof(DAY_TYPE)) {
    //            DAY_TYPE temp = (DAY_TYPE)Enum.Parse(typeof(DAY_TYPE), str, true);
    //            return (T)(object)temp;
    //        }
    //        if (type == typeof(SERIES_TYPE)) {
    //            SERIES_TYPE temp = (SERIES_TYPE)Enum.Parse(typeof(SERIES_TYPE), str, true);
    //            return (T)(object)temp;
    //        }
    //    }
    //    return (T)(object)TIME_TYPE.AFTERNOON;
    //}
}
