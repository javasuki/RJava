package rt;

public class TOut1<T extends Number> extends TOutBase implements IOut, IOutT<T> {

	@Override
	public T getT() {
		return null;
	}

	@Override
	public void DoTest() {
		
	}
	
	public <U> void TestOK(U t) 
	{
		
	}
	
}
