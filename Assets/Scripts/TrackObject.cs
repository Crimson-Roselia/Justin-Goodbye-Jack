using System;
using System.Collections.Generic;
using UnityEngine;

public class TrackObject : MonoBehaviour
{
    [SerializeField] private MidiFileImporter midiFileImporter;
    [Tooltip("对应 MidiFileImporter.midiTracks 中的轨道索引，一条 Lane 对应一条 Track")]
    [SerializeField] private int trackIndex;
    [SerializeField] private NoteSpawner noteSpawner;
    [SerializeField] private KeyCode keyCode;

    public GameObject notePrefab;

    private readonly List<double> timeStamps = new List<double>();
    private readonly List<NoteObject> notes = new List<NoteObject>();

    private int spawnIndex = 0;
    private int inputIndex = 0;
    private bool timestampsReady;

    void Start()
    {
        InitializeTimestamps();
    }

    void InitializeTimestamps()
    {
        if (trackIndex < 0 || trackIndex >= midiFileImporter.midiTracks.Count)
        {
            Debug.LogError(
                $"Lane ({name}): trackIndex={trackIndex} 无效，当前共有 {midiFileImporter.midiTracks.Count} 条轨道");
            return;
        }

        MidiTrack track = midiFileImporter.midiTracks[trackIndex];
        timeStamps.Clear();

        foreach (MidiNote note in track.notes)
        {
            // 长音符轨道只在 Note On 时生成可视音符
            if (track.isLongNote && !note.isNoteOn)
                continue;

            timeStamps.Add(note.timeInSeconds);
        }

        timestampsReady = true;
        Debug.Log($"Lane ({name}): 已绑定轨道 [{trackIndex}] \"{track.trackName}\"，共 {timeStamps.Count} 个音符");
    }

    void Update()
    {
        if (!timestampsReady || timeStamps.Count == 0)
            return;

        if (SongManager.Instance == null || SongManager.Instance.audioSource == null)
            return;

        if (!SongManager.Instance.audioSource.isPlaying)
            return;

        double audioTime = GetInputAudioTime();
        float fallingTime = SongManager.Instance.GetNoteFallingTime();

        // 可能在同一帧内需要生成多个音符
        while (spawnIndex < timeStamps.Count &&
               SongManager.Instance.GetAudioSourceTime() >= timeStamps[spawnIndex] - fallingTime)
        {
            NoteObject noteObject = noteSpawner.SpawnNewNote(trackIndex);
            notes.Add(noteObject);
            spawnIndex++;
        }

        if (inputIndex >= timeStamps.Count)
        {
            // end game
            return;
        }

        double noteTime = timeStamps[inputIndex];
        double margin = SongManager.Instance.marginOfError;

        if (Input.GetKeyDown(keyCode))
        {
            double delta = audioTime - noteTime;

            if (Math.Abs(delta) <= margin)
            {
                Hit();
                inputIndex++;
                return;
            }

            if (delta > margin)
            {
                Miss();
                inputIndex++;
                return;
            }
            // 太早按下：忽略本次输入，等待正确窗口
        }

        // 未按下也会超时 Miss，保证 inputIndex 能推进
        if (audioTime > noteTime + margin)
        {
            Miss();
            inputIndex++;
        }
    }

    private double GetInputAudioTime()
    {
        return SongManager.Instance.GetAudioSourceTime()
               - SongManager.Instance.inputDelayInMilliseconds / 1000.0;
    }

    private void Hit()
    {
        if (inputIndex < notes.Count && notes[inputIndex] != null)
        {
            Destroy(notes[inputIndex].gameObject);
            notes[inputIndex] = null;
        }
    }

    private void Miss()
    {
    }
}
