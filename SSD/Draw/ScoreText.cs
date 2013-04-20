using Microsoft.Xna.Framework;
using System;

namespace SSD
{
    class ScoreText
    {

        public ScoreText(Matrix scorePosition, int scoreAmount)
        {
            _scoreAmount = scoreAmount;
            _scorePosition = scorePosition;
            _initLifespan = 2000;
            _lifespan = _initLifespan;
            _textScale = 1.0f;
        }

        public Matrix getPositionMatrix()
        {
            return _scorePosition;
        }

        public int getScoreAmount()
        {
            return _scoreAmount;
        }

        public void update(GameTime gameTime)
        {
            _lifespan -= gameTime.ElapsedGameTime.Milliseconds;
            _textScale = _lifespan / _initLifespan;
        }

        public float getScale()
        {
            //Scale will start of small, get larger then at middle it will flip
            return HammingWindow(_textScale, 1);
        }

        public bool shouldRemove()
        {
            return _lifespan <= 0;
        }

        //From http://voicerecorder.codeplex.com/SourceControl/changeset/view/f652f98eb1ff#VoiceRecorder.Audio/FftPitchDetector.cs
        //Modified variables to make more sense (I think) and removed the N - 1 and just made it range.
        // http://en.wikipedia.org/wiki/Window_function
        private float HammingWindow(float sampleValue, float range = 1.0f)
        {
            return 0.54f - 0.46f * (float)Math.Cos((2 * Math.PI * sampleValue) / range);
        }

        Matrix _scorePosition;
        int _scoreAmount;
        float _lifespan;
        float _initLifespan;
        float _textScale;
    }
}
