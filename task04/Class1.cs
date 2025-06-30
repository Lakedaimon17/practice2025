using System;

namespace task04
{
    public interface ISpaceship
    {
        void MoveForward();
        void Rotate(int angle);
        void Fire();
        int Speed { get; }
        int FirePower { get; }
    }

    public class Cruiser : ISpaceship
    {
        public int Coordinates { get; private set; } = 0;
        public int RotationAngle { get; private set; } = 0;
        public int Projectiles { get; private set; } = 150;
        public int Speed { get; } = 50;
        public int FirePower { get; } = 100;
        public int Damage { get; private set; } = 0;

        public void MoveForward() => Coordinates += Speed;

        public void Rotate(int angle) => RotationAngle = (RotationAngle + angle) % 360;

        public void Fire()
        {
            if (Projectiles > 0)
            {
                Damage += FirePower;
                Projectiles--;
            }
        }
    }

    public class Fighter : ISpaceship
    {
        public int Coordinates { get; private set; } = 0;
        public int RotationAngle { get; private set; } = 0;
        public int Projectiles { get; private set; } = 50;
        public int Speed { get; } = 100;
        public int FirePower { get; } = 40;
        public int Damage { get; private set; } = 0;

        public void MoveForward() => Coordinates += Speed;

        public void Rotate(int angle) => RotationAngle = (RotationAngle + angle) % 360;

        public void Fire()
        {
            if (Projectiles > 0)
            {
                Damage += FirePower;
                Projectiles--;
            }
        }
    }
}
