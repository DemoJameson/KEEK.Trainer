namespace KEEK.Trainer.Features;

public class MusicSelector : BaseFeature {
    private const string random = "Random";

    private void Awake() {
        Setting.Music.SettingChanged += (_, _) => ToggleMusic();
        Setting.Sound.SettingChanged += (_, _) => ToggleSound();

        HookUtils.Hook(typeof(AudioManager), "Restart", AudioManagerRestart);
        HookUtils.Hook(typeof(AudioManager), "Awake", AudioManagerAwake);
        HookUtils.Hook(typeof(AudioManager), "PlayPlayerSfx", PlayPlayerOrEnvironmentSfx);
        HookUtils.Hook(typeof(AudioManager), "PlayEnviormentSfx", PlayPlayerOrEnvironmentSfx);

        // for hot reload during developing
        if (Setting.MusicSelector == null && AudioManager.Instance != null) {
            InitSetting();
        }
    }

    private void InitSetting() {
        List<string> musics = AudioManager.Instance.CandidateBgms.Select(i => i.NamePrefab.name).ToList();
        musics.Insert(0, random);
        Setting.MusicSelector = Plugin.Instance.Config.Bind("Audio", "Music Selector", musics[0],
            new ConfigDescription("Select the music you like", new AcceptableValueList<string>(musics.ToArray())));
        Setting.MusicSelector.SettingChanged += (_, _) => PlaySelectedMusic();
    }

    private void AudioManagerRestart(Action<AudioManager> orig, AudioManager self) {
        if (Setting.MusicSelector == null) {
            InitSetting();
        }

        if (!Setting.Music.Value) {
            return;
        }

        List<BgmInfo> origCandidateBgms = self.CandidateBgms;

        if (!IsRandomMusic()) {
            // make sure it choose the selected music
            self.CandidateBgms = origCandidateBgms.Where(info => info.NamePrefab.name == Setting.MusicSelector.Value).ToList();
        }

        orig(self);

        self.CandidateBgms = origCandidateBgms;
    }

    private void AudioManagerAwake(Action<AudioManager> orig, AudioManager self) {
        // keep the old audioManager, keep playing the selected music
        if (AudioManager.Instance != null && AudioManager.Instance.NormalBgmSource.isPlaying && !IsRandomMusic()) {
            Destroy(self.gameObject);
            return;
        }

        orig(self);
    }
    
    private void PlayPlayerOrEnvironmentSfx(Action<AudioManager, string> orig, AudioManager self, string name) {
        if (!Setting.Sound.Value) {
            return;
        }

        orig(self, name);
    }
    
    private void ToggleMusic() {
        AudioSource bgmSource = AudioManager.Instance.NormalBgmSource;
        if (!bgmSource.isPlaying && Setting.Music.Value) {
            PlaySelectedMusic();
        } else if (bgmSource.isPlaying && !Setting.Music.Value) {
            if (AudioManager.Instance._playNormalCoroutine is {} coroutine) {
                StopCoroutine(coroutine);
            }

            bgmSource.Stop();
        }
    }

    private void ToggleSound() {
        AudioManager audioManager = AudioManager.Instance;
        if (!Setting.Sound.Value) {
            audioManager.PlayerSfxSource.Stop();
            audioManager.EnviormentSfxSource.Stop();
        }
    }

    private void PlaySelectedMusic() {
        AudioSource bgmSource = AudioManager.Instance.NormalBgmSource;
        List<BgmInfo> bgmInfos = AudioManager.Instance.CandidateBgms;
        bgmSource.Stop();

        BgmInfo bgmInfo;
        if (IsRandomMusic()) {
            bgmInfo = bgmInfos[Random.Range(0, bgmInfos.Count)];
        } else {
            bgmInfo = bgmInfos.First(info => info.NamePrefab.name == Setting.MusicSelector.Value);
        }

        AudioManager.Instance._selectedBgmInfo = bgmInfo;
        bgmSource.clip = bgmInfo.Loop;
        bgmSource.loop = true;

        if (Setting.Music.Value) {
            bgmSource.Play();
        }
    }

    private bool IsRandomMusic() {
        return Setting.MusicSelector.Value == random;
    }
}