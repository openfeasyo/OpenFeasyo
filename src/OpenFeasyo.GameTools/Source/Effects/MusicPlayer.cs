/*
 * The program is developed as a data collection tool in the fields of motion 
 * analysis and physical condition.The user of the software is motivated to 
 * complete exercises through the use of Games. This program is available as
 * a part of the open source project OpenFeasyo found at
 * https://github.com/openfeasyo/OpenFeasyo>.
 * 
 * Copyright (c) 2020 - Lubos Omelina
 * 
 * This program is free software: you can redistribute it and/or modify it 
 * under the terms of the GNU General Public License version 3 as published 
 * by the Free Software Foundation. The Software Source Code is submitted 
 * within i-DEPOT holding reference number: 122388.
 */
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace OpenFeasyo.GameTools.Effects
{
    public class MusicPlayer
    {
        private Dictionary<string, List<Song>> playlists = new Dictionary<string, List<Song>>();
        private int currentSong = 0;
        private string currentPlaylist = "";

        public MusicPlayer()
        {

        }

        public void AddSong(string name, Song song)
        {
            playlists.Add(name, new List<Song>() { song });
        }

        public void Update()
        {
            if (Microsoft.Xna.Framework.Media.MediaPlayer.State != MediaState.Playing)
            {
                Play(currentPlaylist);
            }
            else {
                if (GameTools.Mute)
                {
                    Stop();
                }
            }

        }

        public void Play(string playlist)
        {
            if (!playlists.ContainsKey(playlist)) {
                return;
            }

            if (playlist != currentPlaylist)
            {
                currentPlaylist = playlist;
                currentSong = 0;
                if (Microsoft.Xna.Framework.Media.MediaPlayer.State == MediaState.Playing) {
                    Microsoft.Xna.Framework.Media.MediaPlayer.Stop();
                }
            }

            if (playlists[playlist].Count == 0 || 
                GameTools.Mute ||
                Microsoft.Xna.Framework.Media.MediaPlayer.State == MediaState.Playing) return;
            
            if (playlists[playlist].Count <= currentSong)
            {
                currentSong = 0;
            }


            Microsoft.Xna.Framework.Media.MediaPlayer.IsRepeating = false;
            try
            {
                Microsoft.Xna.Framework.Media.MediaPlayer.Play(playlists[playlist][currentSong]);
            }
            catch (Exception e) {
                Console.WriteLine(e.Message + " \n" + e.StackTrace);
            }

            currentSong++;
        }

        public void Stop()
        {
            if (Microsoft.Xna.Framework.Media.MediaPlayer.State != MediaState.Playing) return;
            Microsoft.Xna.Framework.Media.MediaPlayer.Stop();
        }

        private Dictionary<string, SoundEffect> _sounds = new Dictionary<string, SoundEffect>();

        public void AddSoundEffect(string name, SoundEffect effect)
        {
            _sounds.Add(name, effect);
        }

        public void PlayEffect(string name)
        {
            if (_sounds.ContainsKey(name) && !GameTools.Mute)
            {
                _sounds[name].Play();
            }
        }

        public void Destroy() {
            _sounds.Clear();
            playlists.Clear();
        }

        public Dictionary<string, SoundEffect> Sounds { get { return _sounds; } }
    }
}
