using System;
using System.Collections;
using System.Collections.Generic;

public struct UKTuple<A,B>
{
	public A a;
	public B b;
	
	public UKTuple(A a, B b)
	{
		this.a = a;
		this.b = b;
	}
}

public struct UKTuple<A,B,C>
{
	public A a;
	public B b;
	public C c;
	
	public UKTuple(A a, B b, C c)
	{
		this.a = a;
		this.b = b;
		this.c = c;
	}
}

public struct UKTuple<A,B,C,D>
{
	public A a;
	public B b;
	public C c;
	public D d;
	
	public UKTuple(A a, B b, C c, D d)
	{
		this.a = a;
		this.b = b;
		this.c = c;
		this.d = d;
	}
}
