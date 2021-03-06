﻿using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// An editor window to create new beatmaps.
/// </summary>
public class BeatmapRecorder : EditorWindow
{
    private AudioSource audioSource;
    private NoteRecorder noteRecorder;
    private AudioClip musicClip;

    private SongParser.Metadata songData;

    private string songTitle;
    private string subtitle;
    private string artist;
    private string bannerPath;
    private string backgroundPath;
    private string musicPath;
    private float offset;
    private float sampleStart;
    private float sampleLength;
    private float bpm;

    /// <summary>
    /// Displays the window in the Tools tab.
    /// </summary>
    [MenuItem("Tools/Beatmap Recorder")]
    public static void ShowWindow()
    {
        GetWindow<BeatmapRecorder>("Beatmap Recorder", true);
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Audio Source");
        audioSource = (AudioSource)EditorGUILayout.ObjectField(audioSource, typeof(AudioSource), true);
        EditorGUILayout.EndHorizontal();

        if (audioSource != null)
        {
            if (EditorApplication.isPlaying)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Note Recorder");
                noteRecorder = (NoteRecorder)EditorGUILayout.ObjectField(noteRecorder, typeof(NoteRecorder), true);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Music File");
                musicClip = (AudioClip)EditorGUILayout.ObjectField(musicClip, typeof(AudioClip), false);
                EditorGUILayout.EndHorizontal();

                if (audioSource.clip != musicClip)
                {
                    audioSource.clip = musicClip;
                }

                if (GUILayout.Button("Load Music"))
                {
                    musicPath = EditorUtility.OpenFilePanel("Load Music File", "", "mp3");

                    if (musicPath.Length != 0)
                    {
                        musicClip = LoadMusic(musicPath);
                    }
                }

                if (musicClip != null && noteRecorder != null)
                {
                    DrawMNFileSettings();

                    if (IsMNDataValid())
                    {
                        if (GUILayout.Button("Start Recording"))
                        {
                            songData = new SongParser.Metadata();
                            songData.title = songTitle;
                            songData.subtitle = subtitle;
                            songData.artist = artist;
                            songData.bannerPath = bannerPath;
                            songData.backgroundPath = backgroundPath;
                            songData.musicPath = Path.GetFileName(musicPath);
                            songData.offset = offset;
                            songData.sampleStart = sampleStart;
                            songData.sampleLength = sampleLength;
                            songData.bpm = bpm;

                            string _targetPath = musicPath.Replace(Path.GetFileName(musicPath), "");

                            audioSource.Play();
                            noteRecorder.InitRecording(songData, _targetPath);
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Song title, artist, and BPM must be valid to start recording.", MessageType.Warning);
                    }
                }
                else
                {
                    if (musicClip == null)
                    {
                        EditorGUILayout.HelpBox("Load a music file to start recording.", MessageType.Info);
                    }

                    if (noteRecorder == null)
                    {
                        EditorGUILayout.HelpBox("A Note Recorder from the scene is required.", MessageType.Info);
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Enter Playmode to start recording.", MessageType.Info);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("An Audio Source from the scene is required.", MessageType.Info);
        }
    }

    /// <summary>
    /// Loads and converts music file to the proper format.
    /// </summary>
    /// <param name="path">Path to the music file.</param>
    /// <returns>Converted music file.</returns>
    private AudioClip LoadMusic(string path)
    {
        string _url = string.Format("file://{0}", path);

        WWW _www = new WWW(_url);

        AudioClip _musicClip = NAudioPlayer.FromMp3Data(_www.bytes);
        _musicClip.name = Path.GetFileName(path);
        return _musicClip;
    }

    /// <summary>
    /// Displays the setting fields for the .mn file setup.
    /// </summary>
    private void DrawMNFileSettings()
    {
        EditorGUILayout.LabelField("Song Settings", EditorStyles.boldLabel);

        EditorGUILayout.HelpBox("All song files (music, .mn, banner, and background) must be in the same directory!", MessageType.Info);

        EditorGUILayout.BeginVertical();
        songTitle = EditorGUILayout.TextField("Title", songTitle);
        if (SongParser.IsNullOrWhiteSpace(songTitle))
        {
            EditorGUILayout.HelpBox("Song title cannot be empty or whitespace.", MessageType.Warning);
        }

        subtitle = EditorGUILayout.TextField("Subtitle", subtitle);
        artist = EditorGUILayout.TextField("Artist", artist);
        if (SongParser.IsNullOrWhiteSpace(artist))
        {
            EditorGUILayout.HelpBox("Artist cannot be empty or whitespace.", MessageType.Warning);
        }

        EditorGUILayout.BeginHorizontal();
        bannerPath = EditorGUILayout.TextField("Banner Path", bannerPath);
        if (GUILayout.Button("Change Banner Image"))
        {
            string _bannerPath = EditorUtility.OpenFilePanel("Change Banner Image", "", "*");

            _bannerPath = Path.GetFileName(_bannerPath);
            bannerPath = _bannerPath;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        backgroundPath = EditorGUILayout.TextField("Backgorund Path", backgroundPath);
        if (GUILayout.Button("Change Background Image"))
        {
            string _backgroundPath = EditorUtility.OpenFilePanel("Change Banner Image", "", "*");

            _backgroundPath = Path.GetFileName(_backgroundPath);
            backgroundPath = _backgroundPath;
        }
        EditorGUILayout.EndHorizontal();

        offset = EditorGUILayout.FloatField("Offset", offset);

        EditorGUILayout.LabelField("Song Length: " + musicClip.length);

        sampleStart = EditorGUILayout.Slider("Sample Start Time", sampleStart, 0.0f, musicClip.length);
        sampleLength = EditorGUILayout.Slider("Sample Length Time", sampleLength, 0.0f, musicClip.length - sampleStart);
        bpm = EditorGUILayout.FloatField("BPM", bpm);
        if (bpm <= 0.0f)
        {
            EditorGUILayout.HelpBox("Invalid BPM.", MessageType.Error);
        }

        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Checks the validity of the .mn setup settings.
    /// </summary>
    /// <returns>True or false.</returns>
    private bool IsMNDataValid()
    {
        if (SongParser.IsNullOrWhiteSpace(songTitle))
        {
            return false;
        }
        if (SongParser.IsNullOrWhiteSpace(artist))
        {
            return false;
        }
        if (bpm <= 0.0f)
        {
            return false;
        }
        return true;
    }
}
