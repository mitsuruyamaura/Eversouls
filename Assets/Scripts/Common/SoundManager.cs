﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

// 音管理クラス
public class SoundManager : MonoBehaviour {

    protected static SoundManager instance;

    public static SoundManager Instance {
        get {
            if (instance == null) {
                instance = (SoundManager)FindObjectOfType(typeof(SoundManager));

                if (instance == null) {
                    Debug.LogError("SoundManager Instance Error");
                }
            }
            return instance;
        }
    }

    // 音楽管理
    public enum ENUM_BGM : int {
        TITLE,
        MAIN,
        MATCHING,
        BATTLE,
        RESULT,
        LAST_ONE_MINUTE,
    }

    // 効果音管理
    public enum ENUM_SE : int {
        BTN_OK,
        BTN_NO,
        RELEASE_OK,
        RELEASE_NG,
        DISCOVERY_TRAP,
        RELEASE_CHECK,    // 矢印を止めた時のSE
        ITEMBOX_TRAP_GET, // アイテムボックスからトラップ入手時
        HEAL,
        GET_GEM,
        READYGO,
        LEVELUP,
        SPEEDUP,
        RANKDOWN,
        TRAP_SET,
        MISSILE_SET,
        SUPER_MODE,
        LOBBY_USER_IN,
        LOBBY_MAX_READY,
        LOBBY_START,
    }

    // ボイス
    public enum ENUM_VOICE : int {


    }

    // クロスフェード時間
    public const float XFADE_TIME = 1.4f;

    // 音量
    public SoundVolume volume = new SoundVolume();

    // === AudioSource ===
    // BGM
    private AudioSource[] BGMsources = new AudioSource[2];
    // SE
    private AudioSource[] SEsources = new AudioSource[16];
    // 音声
    private AudioSource[] VoiceSources = new AudioSource[16];

    // === AudioClip ===
    // BGM
    public BGMDatas[] BGM;
    // SE
    public AudioClip[] SE;
    // 音声
    public AudioClip[] Voice;

    // SE用AudioMixer
    public AudioMixer audioMixer;

    bool isXFading;

    int currentBgmIndex = 999;

    [System.Serializable]
    public class BGMDatas {
        public AudioClip clip;
        public float loopTime;
        public float endTime;
    }

    void Awake() {
        GameObject[] obj = GameObject.FindGameObjectsWithTag("SoundManager");

        if (obj.Length > 1) {
            // 既に存在しているなら削除
            Destroy(gameObject);
        } else {
            // 音管理はシーン遷移では破棄させない
            DontDestroyOnLoad(gameObject);
        }

        // BGM AudioSource
        BGMsources[0] = gameObject.AddComponent<AudioSource>();
        BGMsources[1] = gameObject.AddComponent<AudioSource>();

        // SE AudioSource
        for (int i = 0; i < SEsources.Length; i++) {
            SEsources[i] = gameObject.AddComponent<AudioSource>();
        }

        // 音声 AudioSource
        for (int i = 0; i < VoiceSources.Length; i++) {
            VoiceSources[i] = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update() {
        // ミュート設定
        BGMsources[0].mute = volume.Mute;
        BGMsources[1].mute = volume.Mute;
        foreach (AudioSource source in SEsources) {
            source.mute = volume.Mute;
        }
        foreach (AudioSource source in VoiceSources) {
            source.mute = volume.Mute;
        }

        // ボリューム設定
        if (!isXFading) {
            BGMsources[0].volume = volume.BGM;
            BGMsources[1].volume = volume.BGM;
        }
        foreach (AudioSource source in SEsources) {
            source.volume = volume.SE;
        }
        foreach (AudioSource source in VoiceSources) {
            source.volume = volume.Voice;
        }

        // Loop処理
        if (currentBgmIndex != 999) {
            if (BGM[currentBgmIndex].loopTime > 0f) {
                if (!BGMsources[0].mute && BGMsources[0].isPlaying && BGMsources[0].clip != null) {
                    if (BGMsources[0].time >= BGM[currentBgmIndex].endTime) {
                        BGMsources[0].time = BGM[currentBgmIndex].loopTime;
                    }
                }
                if (!BGMsources[1].mute && BGMsources[1].isPlaying && BGMsources[1].clip != null) {
                    if (BGMsources[1].time >= BGM[currentBgmIndex].endTime) {
                        BGMsources[1].time = BGM[currentBgmIndex].loopTime;
                    }
                }
            }
        }
    }

    // ***** BGM再生 *****
    // BGM再生
    public void PlayBGM(ENUM_BGM bgmNo, bool loopFlg = true) {
        int index = (int)bgmNo;
        currentBgmIndex = index;
        //if(PlayerPrefs.GetInt(Constant.BGM_FLG_NAME,1) == 1){
        if (0 > index || BGM.Length <= index) {
            return;
        }
        // 同じBGMの場合は何もしない
        if (BGMsources[0].clip != null && BGMsources[0].clip == BGM[index].clip) {
            return;
        } else if (BGMsources[1].clip != null && BGMsources[1].clip == BGM[index].clip) {
            return;
        }
        volume.BGM = GameData.instance.volumeBGM;
        // フェードでBGM開始
        if (BGMsources[0].clip == null && BGMsources[1].clip == null) {
            BGMsources[0].loop = loopFlg;
            BGMsources[0].clip = BGM[index].clip;
            BGMsources[0].Play();
            BGMsources[0].DOFade(GameData.instance.volumeBGM, XFADE_TIME);
        } else {
            // クロスフェード
            StartCoroutine(CrossfadeChangeBMG(index, loopFlg));
        }
    }

    private IEnumerator CrossfadeChangeBMG(int index, bool loopFlg) {
        isXFading = true;
        if (BGMsources[0].clip != null) {
            // 0がなっていて、1を新しい曲としてPlay
            BGMsources[1].volume = 0;
            BGMsources[1].clip = BGM[index].clip;
            BGMsources[1].loop = loopFlg;
            BGMsources[1].Play();
            BGMsources[0].DOFade(0, XFADE_TIME).SetEase(Ease.Linear);
            BGMsources[1].DOFade(GameData.instance.volumeBGM, XFADE_TIME).SetEase(Ease.Linear);
            yield return new WaitForSeconds(XFADE_TIME);
            BGMsources[0].Stop();
            BGMsources[0].clip = null;
        } else {
            // 1がなっていて、0を新しい曲としてPlay
            BGMsources[0].volume = 0;
            BGMsources[0].clip = BGM[index].clip;
            BGMsources[0].loop = loopFlg;
            BGMsources[0].Play();
            BGMsources[1].DOFade(0, XFADE_TIME).SetEase(Ease.Linear);
            BGMsources[0].DOFade(GameData.instance.volumeBGM, XFADE_TIME).SetEase(Ease.Linear);
            yield return new WaitForSeconds(XFADE_TIME);
            BGMsources[1].Stop();
            BGMsources[1].clip = null;
        }
        isXFading = false;
    }

    // BGM停止
    public void StopBGM() {
        BGMsources[0].Stop();
        BGMsources[1].Stop();
        BGMsources[0].clip = null;
        BGMsources[1].clip = null;
    }

    // ***** SE再生 *****
    // SE再生
    public void PlaySE(ENUM_SE seNo) {
        int index = (int)seNo;
        //if(PlayerPrefs.GetInt(Constant.SE_FLG_NAME,1) == 1){
        if (0 > index || SE.Length <= index) {
            return;
        }

        // 再生中で無いAudioSouceで鳴らす
        foreach (AudioSource source in SEsources) {
            if (false == source.isPlaying) {
                source.clip = SE[index];
                volume.SE = GameData.instance.volumeSE;
                source.Play();
                return;
            }
        }
        //}
    }

    // SE停止
    public void StopSE() {
        // 全てのSE用のAudioSouceを停止する
        foreach (AudioSource source in SEsources) {
            source.Stop();
            source.clip = null;
        }
    }

    public void SetAudioMixerVolume(float vol) {
        if (vol == 0) {
            audioMixer.SetFloat("volumeSE", -80);
        } else {
            audioMixer.SetFloat("volumeSE", 0);
        }
    }

    public void MuteBGM() {
        BGMsources[0].Stop();
        BGMsources[1].Stop();
    }

    public void ResumeBGM() {
        BGMsources[0].Play();
        BGMsources[1].Play();
    }

    // ***** 音声再生 *****
    // 音声再生
    //public void PlayVoice(ENUM_VOICE voiceNo){
    //	int index = (int)voiceNo;
    //	if(PlayerPrefs.GetInt(Constant.VOICE_FLG_NAME,1) == 1){
    //		if( 0 > index || Voice.Length <= index ){
    //			return;
    //		}
    //		// 再生中で無いAudioSouceで鳴らす
    //		foreach(AudioSource source in VoiceSources){
    //			if( false == source.isPlaying ){
    //				source.clip = Voice[index];
    //				volume.Voice = gameData.volumeVoice;
    //				source.Play();
    //				return;
    //			}
    //		}
    //       }
    //   }

    //   // 音声停止
    //   public void StopVoice(){
    //	// 全ての音声用のAudioSouceを停止する
    //	foreach(AudioSource source in VoiceSources){
    //		source.Stop();
    //		source.clip = null;
    //	}  
    //}
}
