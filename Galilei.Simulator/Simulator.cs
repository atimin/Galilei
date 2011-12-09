using System;

using Galilei.Core;

namespace Galilei.Simulator
{
	[Node]
	public class Simulator : Engine
	{
		public Simulator() : base()
		{
		}
		
		public override void OnScan ()
		{
			Random rand = new Random();
			foreach (Point point in Points) {
				if (point is RandomPoint) {
					RandomPoint rPoint = point as RandomPoint;
					rPoint.SetValue(rand.Next(rPoint.MinValue, rPoint.MaxValue));
				}
			}
		}
	}
}

