using System;
using System.Timers;
using System.Collections.Generic;

namespace Galilei.Core
{
	public class Engine : Node
	{
		private double scanRate;
		private Timer timer;
		private List<Point> points;
		
		[Config]
		public double ScanRate 
		{
			get { return scanRate; }
			set 
			{ 
				scanRate = value; 
				timer.Interval = scanRate * 1000;
			}
		}
		
		public List<Point> Points 
		{
			get { return points; }
		}
		
		public Engine ()
		{
			scanRate = 5;
			points = new List<Point>();
			
			//Init timer
			timer = new Timer(scanRate * 1000);
			timer.Elapsed += HandleTimerElapsed;
			timer.AutoReset = true;
			timer.Start();
		}
		

		void HandleTimerElapsed (object sender, ElapsedEventArgs e)
		{
			OnScan();
		}				
		
		public virtual void OnScan() {}
	}
}

