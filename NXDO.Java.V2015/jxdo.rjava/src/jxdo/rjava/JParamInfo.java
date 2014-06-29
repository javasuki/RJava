package jxdo.rjava;

public class JParamInfo {
	
	
	/**
	 * 泛型参数时,擦除后可替代的超类类型
	 * @return
	 */
	public Class<?> getTypeClass(){
		return cls;
	}
	
	Class<?> cls;
	public void setTypeClass(Class<?> clazz){
		cls = clazz;
	}
	
	/**
	 * 泛型参数时,为参数名称,比如E,T等
	 * @return
	 */
	public String getGenericParamName(){
		return _GenericParamName;
	}
	
	String _GenericParamName;
	void setGenericParamName(String gpn){
		this._GenericParamName = gpn;
	}
	
	/**
	 * true 参数为参数泛型参数
	 */
	public boolean IsGeneric;
	
	/**
	 * true: 参数为 泛型类型
	 */
	public boolean IsGenericType;
}
