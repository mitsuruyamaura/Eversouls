﻿/// <summary>
/// 罠タイプ一覧
/// 一部は宝箱の罠にも流用
/// 解除ありなし/回避ありなし/ダメージありなし/状態異常ありなし/そのほかペナルティありなし/解除回避成功時のEXPと宝物ランクへのボーナス/
/// </summary>
public enum TRAP_TYPE {
    落盤,
    シュート,
    落とし穴,
    矢の雨,              // 弓矢が大量に飛んでくる。回避が難しい。失敗するとダメージ
    落石,
    転移装置,            // 別のエリアへランダムに強制移動
    爆発,
    スリップ,
    裂け目,
    水没,
    炎の渦の幻,
    つむじ風,
    幻覚の壁,
    たどり着けない階段,
    石つぶて,
    電撃,
    呪い,                 // スキル使用不可
    毒針,                 // 解除に失敗すると毒状態
    スキルドレイン,       // 仲間の残りスキル回数-1
    ヴィジョンドレイン,   // ヴィジョンポイント減少
    亡霊の群れ,           // AP減少
    アイテム破壊,         // 装備しているアイテムを１つ破壊
    見えざる盗人,         // 取得している宝物を１つ分消失,
    記憶喪失,             // 進捗度0
    陥穽,                 // ブービートラップ。槍のついている落とし穴
    砂嵐,                 // 方向感覚のスキルがないと、迷子になって進捗度が進まなくなる
    砂埃,
    煙幕,
    混乱,                 // 知覚系スキルが使用不可
    鉛の体,               // 運動系スキルが使用不可。水でおぼれる
    倒木,
    COUNT,
}
