using Godot;

namespace RandomDungeons
{
    public class GarbageCollectorMeter : ProgressBar
    {
        private ulong _lastMemUsage;
        private ulong _lastPeakMemUsage;
        private ulong _lastMinMemUsage;

        private int _gcCount = 0;

        public override void _Process(float delta)
        {
            ulong memUsage = OS.GetStaticMemoryUsage();

            if (memUsage < _lastMemUsage)
            {
                _gcCount++;

                _lastPeakMemUsage = _lastMemUsage;
                _lastMinMemUsage = memUsage;
            }

            if (memUsage > _lastPeakMemUsage)
                _lastPeakMemUsage = memUsage;

            MaxValue = _lastPeakMemUsage - _lastMinMemUsage;
            Value = memUsage - _lastMinMemUsage;

            _lastMemUsage = memUsage;
        }
    }
}
