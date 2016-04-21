﻿using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Animations
{
    public class KeyFrameAnimation : IUpdate
    {
        public KeyFrameAnimation(string name, TextureRegion2D[] keyFrames, float frameDuration = 0.2f,
            bool isLooping = true, bool isReversed = false, bool isPingPong = false)
        {
            Name = name;
            KeyFrames = keyFrames;
            FrameDuration = frameDuration;
            IsLooping = isLooping;
            IsReversed = isReversed;
            IsPingPong = isPingPong;
            CurrentFrameIndex = IsReversed ? KeyFrames.Length - 1 : 0;
            IsPaused = false;
        }

        public KeyFrameAnimation(string name, TextureRegion2D[] keyFrames, KeyFrameAnimationData data)
            : this(name, keyFrames, data.FrameDuration, data.IsLooping, data.IsReversed, data.IsPingPong)
        {
        }

        public string Name { get; }
        public TextureRegion2D[] KeyFrames { get; }
        public float FrameDuration { get; set; }
        public bool IsLooping { get; set; }
        public bool IsReversed { get; set; }
        public bool IsPingPong { get; set; }
        public bool IsComplete => _currentTime >= AnimationDuration;
        public bool IsPlaying => !IsPaused && !IsComplete;
        public bool IsPaused { get; private set; }
        public float AnimationDuration => IsPingPong
            ? (KeyFrames.Length * 2 - 2) * FrameDuration
            : KeyFrames.Length * FrameDuration;
        public TextureRegion2D CurrentFrame => KeyFrames[CurrentFrameIndex];
        public int CurrentFrameIndex { get; private set; }

        public Action OnCompleted { get; set; }

        private float _currentTime;

        public void Play()
        {
            IsPaused = false;
        }

        public void Pause()
        {
            IsPaused = true;
        }

        public void Stop()
        {
            Pause();
            Rewind();
        }

        public void Rewind()
        {
            _currentTime = 0;
        }

        public void Update(GameTime gameTime)
        {
            Update(gameTime.GetElapsedSeconds());
        }

        public void Update(float deltaTime)
        {
            if (!IsPlaying)
                return;

            _currentTime += deltaTime;

            if (IsComplete)
            {
                OnCompleted?.Invoke();

                if (IsLooping)
                    _currentTime -= AnimationDuration;
            }

            if (KeyFrames.Length == 1)
            {
                CurrentFrameIndex = 0;
                return;
            }

            var frameIndex = (int)(_currentTime / FrameDuration);
            var length = KeyFrames.Length;

            if (IsPingPong)
            {
                frameIndex = frameIndex % (length * 2 - 2);

                if (frameIndex >= length)
                    frameIndex = length - 2 - (frameIndex - length);
            }

            if (IsLooping)
            {

                if (IsReversed)
                {
                    frameIndex = frameIndex % length;
                    frameIndex = length - frameIndex - 1;
                }
                else
                {
                    frameIndex = frameIndex % length;
                }
            }
            else
            {
                frameIndex = IsReversed ? Math.Max(length - frameIndex - 1, 0) : Math.Min(length - 1, frameIndex);
            }

            CurrentFrameIndex = frameIndex;
        }
    }
}
