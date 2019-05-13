﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteRecorder : MonoBehaviour
{
    public float noteSpeed = 0.009f;

    private bool isInit = false;
    private SongParser.Metadata songData;
    private float songTimer = 0.0f;
    private float barTime = 0.0f;
    private float barExecutedTime = 0.0f;
    private GameObject player;
    private AudioSource audioSource;
    private SongParser.NoteData noteData;
    private int barCount = 0;

    private string targetPath;
    private bool isRecordingBar = false;
    private bool isRecordingNotes = false;

    // Start is called before the first frame update
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        audioSource = player.GetComponent<AudioSource>();
    }

    public void InitRecording(SongParser.Metadata _songData, string _targetPath)
    {
        barTime = (60.0f / _songData.bpm) * 4.0f;

        songData = _songData;
        targetPath = _targetPath;

        StartCoroutine(Record());
    }

    private IEnumerator Record()
    {
        Debug.Log("Recording begun.");
        noteData = new SongParser.NoteData();
        noteData.bars = new List<List<SongParser.Notes>>();

        while (audioSource.isPlaying)
        {
            float _timeOffset = 10.0f / (noteSpeed / Time.deltaTime);

            songTimer = audioSource.time;

            Debug.Log(songTimer - _timeOffset);

            if (!isRecordingBar && (songTimer - _timeOffset >= (barExecutedTime - barTime)))
            {
                StartCoroutine(RecordBar());

                barExecutedTime += barTime;
            }

            yield return null;
        }

        songData.noteData = this.noteData;

        SongWriter _songWriter = new SongWriter();
        _songWriter.Write(songData, targetPath);

        yield break;
    }

    private IEnumerator RecordBar()
    {
        Debug.Log("New bar.");
        isRecordingBar = true;
        List<SongParser.Notes> _bar = new List<SongParser.Notes>();

        int _barCounter = 0;

        while (_barCounter < 4)
        {
            if (!isRecordingNotes)
            {
                StartCoroutine(RecordNotes(_bar));
                Debug.Log("Record notes called.");
                _barCounter++;
            }

            yield return null;
        }

        noteData.bars.Add(_bar);

        isRecordingBar = false;
        yield break;
    }

    private IEnumerator RecordNotes(List<SongParser.Notes> bar)
    {
        Debug.Log("New notes.");
        isRecordingNotes = true;
        SongParser.Notes _notes = new SongParser.Notes();

        _notes.top = 0;
        _notes.middle = 0;
        _notes.bottom = 0;

        float _timer = 0.0f;

        //while (_timer <= (barTime / 4) - Time.deltaTime)
        //{
        //    if (Input.GetKeyDown(KeyCode.W))
        //    {
        //        AddNotes(ref _notes, SongParser.NoteType.Fire);
        //    }
        //    else if (Input.GetKeyDown(KeyCode.A))
        //    {
        //        AddNotes(ref _notes, SongParser.NoteType.Air);
        //    }
        //    else if (Input.GetKeyDown(KeyCode.D))
        //    {
        //        AddNotes(ref _notes, SongParser.NoteType.Water);
        //    }
        //    else if (Input.GetKeyDown(KeyCode.S))
        //    {
        //        AddNotes(ref _notes, SongParser.NoteType.Earth);
        //    }
        //    _timer += Time.deltaTime;
        //    yield return null;
        //}

        for (int i = 0; i < 4; i++)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                AddNotes(ref _notes, SongParser.NoteType.Fire);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                AddNotes(ref _notes, SongParser.NoteType.Air);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                AddNotes(ref _notes, SongParser.NoteType.Water);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                AddNotes(ref _notes, SongParser.NoteType.Earth);
            }

            yield return new WaitForSeconds((barTime / 4) - Time.deltaTime);
        }

        bar.Add(_notes);

        isRecordingNotes = false;
        yield break;
    }

    private void AddNotes(ref SongParser.Notes notes, SongParser.NoteType noteType)
    {
        int _lane = 1;

        if (player.transform.position.y > 0.0f && player.transform.position.y <= 0.5f)
        {
            _lane = 0;
        }
        else if (player.transform.position.y < 0.0f && player.transform.position.y >= -0.5f)
        {
            _lane = 2;
        }

        switch (_lane)
        {
            case 0:
                notes.top = noteType;
                break;
            case 1:
                notes.middle = noteType;
                break;
            case 2:
                notes.bottom = noteType;
                break;
        }
    }
}
