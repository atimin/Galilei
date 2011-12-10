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
					RandomPoint rp = point as RandomPoint;
					rp.SetValue(rp.MinValue + rand.NextDouble()*(rp.MaxValue - rp.MinValue));
				}
			}
		}
	}
}

