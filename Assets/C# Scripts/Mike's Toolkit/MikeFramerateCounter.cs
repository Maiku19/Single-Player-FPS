using TMPro;
using UnityEngine;

namespace Mike
{
	public class MikeFramerateCounter : MonoBehaviour
	{
		[System.Serializable]
		public enum Mode
		{
			Current,
			Average,
			Max,
			Min
		}

		[SerializeField] TextMeshProUGUI _framerateText;
		[SerializeField, Tooltip("Determines how FPS will be evaluated")] Mode _mode = Mode.Average;
		[SerializeField, Tooltip("After how much time in seconds must pass before counter should refresh")] float _refreshDelay = .5f;
        [SerializeField] string _prefix;
        [SerializeField] string _suffix = " FPS";

		float _startTime = 0;

        float _frameTimeSum = 0;
        ushort _frameCount = 0;

        float? _evaluator;

        private void Update()
        {
            switch(_mode)
			{
				case Mode.Current: FramerateCurrent(); break;
				case Mode.Average: FramerateAverage(); break;
                case Mode.Max: FramerateMax(); break;
                case Mode.Min: FramerateMin(); break;
			}
        }

        void SetText(string text)
        {
            _framerateText.SetText(_prefix + text + _suffix);
        }

		void FramerateCurrent()
		{
            SetText((1 / Time.unscaledDeltaTime).ToString("#.00"));
        }

        void FramerateAverage()
		{
			_frameCount++;
			_frameTimeSum += Time.unscaledDeltaTime;

			if(Time.unscaledTime - _startTime >= _refreshDelay)
			{
				//Display
				SetText((1 / (_frameTimeSum / _frameCount)).ToString("#.00"));

				//Reset
				_frameCount = 0;
				_frameTimeSum = 0;
				_startTime = Time.unscaledTime;
			}
		}

        void FramerateMax()
        {
			_evaluator ??= Time.unscaledDeltaTime;

			if(_evaluator.Value > Time.unscaledDeltaTime) { _evaluator = Time.unscaledDeltaTime; }

            if (Time.unscaledTime - _startTime >= _refreshDelay)
            {
                //Display
                SetText((1 /_evaluator.Value).ToString("#.00"));

                //Reset
                _evaluator = null;
                _startTime = Time.unscaledTime;
            }
        }

        void FramerateMin()
        {
            _evaluator ??= Time.unscaledDeltaTime;

            if (_evaluator.Value < Time.unscaledDeltaTime) { _evaluator = Time.unscaledDeltaTime; }

            if (Time.unscaledTime - _startTime >= _refreshDelay)
            {
                //Display
                SetText((1 / _evaluator.Value).ToString("#.00"));

                //Reset
                _evaluator = null;
                _startTime = Time.unscaledTime;
            }
        }
    }
}