﻿using Dissonance.Audio.Playback;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace Dissonance.Editor
{
    [CustomEditor(typeof(SamplePlaybackComponent))]
    [UsedImplicitly]
    public class SamplePlaybackComponentEditor
        : UnityEditor.Editor
    {
        private Texture2D _logo;

        private readonly AnimationCurve _rateGraph = new AnimationCurve();
        private float _nextRateGraphKey;

        public void Awake()
        {
            _logo = Resources.Load<Texture2D>("dissonance_logo");
        }

        public override bool RequiresConstantRepaint()
        {
            return Application.isPlaying;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Label(_logo);

            if (Application.isPlaying)
            {
                var component = (SamplePlaybackComponent)target;
                var maybeSession = component.Session;
                if (maybeSession != null)
                {
                    var session = maybeSession.Value;
                    var sync = session.SyncState;

                    EditorGUILayout.LabelField($"Buffered Packets: {session.BufferCount}");

                    EditorGUILayout.LabelField($"Playback Position: {sync.ActualPlaybackPosition.TotalSeconds:0.00}s");
                    EditorGUILayout.LabelField($"Ideal Position: {sync.IdealPlaybackPosition.TotalSeconds:0.00}s");
                    EditorGUILayout.LabelField($"Desync: {sync.Desync.TotalMilliseconds:0.0}ms");
                    EditorGUILayout.LabelField($"Compensated Playback Speed: {sync.CompensatedPlaybackSpeed:P1}");

                    _rateGraph.AddKey(_nextRateGraphKey++, sync.CompensatedPlaybackSpeed);
                    while (_rateGraph.length > 200)
                        _rateGraph.RemoveKey(0);
                    EditorGUILayout.CurveField(_rateGraph, GUILayout.Height(100));
                }
                else
                {
                    EditorGUILayout.LabelField("Not Speaking");

                    // Clear the data from the buffer graph
                    while (_rateGraph.length > 0)
                        _rateGraph.RemoveKey(_rateGraph.length - 1);
                    _nextRateGraphKey = 0;
                }
            }
        }
    }
}