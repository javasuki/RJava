package jxdo.rjava;

public class JParamInfo {
	
	
	/**
	 * ���Ͳ���ʱ,�����������ĳ�������
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
	 * ���Ͳ���ʱ,Ϊ��������,����E,T��
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
	 * true ����Ϊ�������Ͳ���
	 */
	public boolean IsGeneric;
	
	/**
	 * true: ����Ϊ ��������
	 */
	public boolean IsGenericType;
}
