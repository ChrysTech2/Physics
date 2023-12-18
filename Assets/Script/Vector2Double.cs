using System;
using UnityEngine;

public struct Vector2Double{

	public double x, y;

	public Vector2Double(double x = 0, double y = 0){
		this.x = x; this.y = y;
	}

	public double magnitude{
		get{
			return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
		}
		set{
			this = direction * value;
		}
	}

	public Vector2Double direction{
		get{
			double mag = magnitude;
			if (mag != 0)
				return this / mag;
			return zero;
		}
		set{
			this = value.direction * magnitude;
		}
	}

	public static double Distance(Vector2Double a, Vector2Double b){
		return Math.Sqrt(Math.Pow(a.x - b.x, 2) + Math.Pow(a.y - b.y, 2));
	}

	public static Vector2Double operator+(Vector2Double a, Vector2Double b){
		return new Vector2Double(a.x + b.x, a.y + b.y);
	}

	public static Vector2Double operator-(Vector2Double a, Vector2Double b){
		return new Vector2Double(a.x - b.x, a.y - b.y);
	}

	public static Vector2Double operator*(Vector2Double a, double b){
		return new Vector2Double(a.x * b, a.y * b);
	}

	public static Vector2Double operator/(Vector2Double a, double b){
		return new Vector2Double(a.x / b, a.y / b);
	}

	public static Vector2Double operator*(double a, Vector2Double b){
		return new Vector2Double(a * b.x, a * b.y);
	}

	public static Vector2Double operator/(double a, Vector2Double b){
		return new Vector2Double(a / b.x, a / b.y);
	}

	public Vector2 ToVector2(){
		return new Vector2((float)x, (float)y);
	}

	public Vector3 ToVector3(){
		return new Vector3((float)x, (float)y);
	}

	public static Vector2Double zero{
		get{return new Vector2Double(0, 0);}
	}

	public static Vector2Double one{
		get{return new Vector2Double(1, 1);}
	}

	public static Vector2Double up{
		get{return new Vector2Double(0, 1);}
	}

	public static Vector2Double down{
		get{return new Vector2Double(0, -1);}
	}

	public static Vector2Double right{
		get{return new Vector2Double(1, 0);}
	}

	public static Vector2Double left{
		get{return new Vector2Double(-1, 0);}
	}

	public override string ToString(){
		return $"({x}, {y})".Replace(',', '.');
	}

	public double ToRadians(){
		Vector2Double dir = direction;
		return Math.Atan2(dir.y, dir.x);
	}

	public double ToDegrees(){
		Vector2Double dir = direction;
		return Math.Atan2(dir.y, dir.x) * 180 / Math.PI;
	}
}
