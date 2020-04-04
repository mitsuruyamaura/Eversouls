using UnityEngine;
using System.IO;
using System.Collections;

public class ShareController {

    public static IEnumerator ShareScreenShot() {
        //ファイル名が重複しないように実行時間を付与。
        //string fileName = System.DateTime.Now.ToString("Screen yyyy-MM-dd HH.mm.ss") + ".png";

        //スクリーンショット画像の保存先を設定。
        string imagePath = Application.persistentDataPath + "/image.png";    //"/" + fileName;

        // 前回のデータを削除
        //File.Delete(imagePath);

        //スクリーンショットを撮影
        ScreenCapture.CaptureScreenshot("image.png");

        yield return new WaitForEndOfFrame();

        // 撮影画像の保存が完了するまで待機
        //while (true) {
        //    if (File.Exists(imagePath)) break;
        //    yield return null;
        //}

        // アプリやシーンごとにShareするメッセージを設定
        string text = "ツイート内容\n#hashtag ";
        string URL = "url";

        yield return new WaitForSeconds(1.0f);

        // Share
        SocialConnector.SocialConnector.Share(text, URL, imagePath);
    }
}