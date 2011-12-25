using NUnit.Framework;
using System;

namespace Galilei.Core
{
	[TestFixture()]
	public class UPoint
	{
		private Point point;
		private Engine engine;
		
		[SetUp]
		public void SetUp()
		{
			point =  new Point();
			engine = new Engine();
			point.Engine = engine;
		}
	
		[Test]
		public void TestEngine()
		{
			Assert.AreEqual(engine, point.Engine);
			Assert.AreEqual(point, engine.Points[0]);
		}
		
		[Test]
		public void TestValue()
		{
			Assert.IsNotNull(point.GetValue());
			point.SetValue(10);
			
			Assert.AreEqual(10, point.GetValue());
			Assert.AreEqual(10, point.Value);
		}
		
		[Test]
		public void TestQuality()
		{
			Assert.AreEqual(Quality.Init, point.Quality);
			
			point.SetValue(10);
			Assert.AreEqual(Quality.Good, point.Quality);
			
			point.SetValue(Quality.Bad);
			Assert.AreEqual(Quality.Bad, point.Quality);
		}
		
		[Test]
		public void TestTimeStamp()
		{
			point.SetValue(new DateTime(1000));
			Assert.AreEqual(new DateTime(1000), point.Timestamp);
		}
	}
}

