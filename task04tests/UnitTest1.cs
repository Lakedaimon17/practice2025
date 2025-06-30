using Xunit;
using task04;

namespace task04tests
{
    public class SpaceshipTests
    {
        [Fact]
        public void Cruiser_ShouldHaveCorrectStats()
        {
            var cruiser = new Cruiser();
            Assert.Equal(0, cruiser.Coordinates);
            Assert.Equal(0, cruiser.RotationAngle);
            Assert.Equal(150, cruiser.Projectiles);
            Assert.Equal(50, cruiser.Speed);
            Assert.Equal(100, cruiser.FirePower);
            Assert.Equal(0, cruiser.Damage);
        }

        [Fact]
        public void Fighter_ShouldHaveCorrectStats()
        {
            var fighter = new Fighter();
            Assert.Equal(0, fighter.Coordinates);
            Assert.Equal(0, fighter.RotationAngle);
            Assert.Equal(50, fighter.Projectiles);
            Assert.Equal(100, fighter.Speed);
            Assert.Equal(40, fighter.FirePower);
            Assert.Equal(0, fighter.Damage);
        }

        [Fact]
        public void Fighter_ShouldBeFasterThanCruiser()
        {
            var fighter = new Fighter();
            var cruiser = new Cruiser();
            Assert.True(fighter.Speed > cruiser.Speed);
        }

        [Fact]
        public void Cruiser_ShouldBeStrongerThanFighter()
        {
            var fighter = new Fighter();
            var cruiser = new Cruiser();
            Assert.True(cruiser.FirePower > fighter.FirePower);
        }

        [Fact]
        public void Cruiser_MovesForwardCorrect()
        {
            var cruiser = new Cruiser();
            int initialCoordinate = cruiser.Coordinates;
            cruiser.MoveForward();
            Assert.Equal(initialCoordinate + cruiser.Speed, cruiser.Coordinates);
        }

        [Fact]
        public void Cruiser_RotatesCorrect()
        {
            var cruiser = new Cruiser();
            int initialRotation = cruiser.RotationAngle;
            cruiser.Rotate(50);
            Assert.Equal(50, cruiser.RotationAngle);
            cruiser.Rotate(50);
            Assert.Equal(100, cruiser.RotationAngle);
        }

        [Fact]
        public void Cruiser_FiresCorrect()
        {
            var cruiser = new Cruiser();
            int initialProjectiles = cruiser.Projectiles;
            int initialDamage = cruiser.Damage;
            cruiser.Fire();
            Assert.Equal(initialProjectiles - 1, cruiser.Projectiles);
            Assert.Equal(initialDamage + cruiser.FirePower, cruiser.Damage);
        }

        [Fact]
        public void Fighter_MovesForwardCorrect()
        {
            var fighter = new Fighter();
            int initialCoordinate = fighter.Coordinates;
            fighter.MoveForward();
            Assert.Equal(initialCoordinate + fighter.Speed, fighter.Coordinates);
        }

        [Fact]
        public void Fighter_RotatesCorrect()
        {
            var fighter = new Fighter();
            fighter.Rotate(50);
            Assert.Equal(50, fighter.RotationAngle);
            fighter.Rotate(360);
            Assert.Equal(50, fighter.RotationAngle);
        }

        [Fact]
        public void Fighter_FiresCorrect()
        {
            var fighter = new Fighter();
            int initialProjectiles = fighter.Projectiles;
            int initialDamage = fighter.Damage;
            fighter.Fire();
            Assert.Equal(initialProjectiles - 1, fighter.Projectiles);
            Assert.Equal(initialDamage + fighter.FirePower, fighter.Damage);
        }
    }
}
