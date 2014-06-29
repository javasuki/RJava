#include "jvmrun.h"
#include "InterfaceDefine.h"
#include "lconvert.h"
#include "larray.h"
#include "ValueTypeFlag.h"

#pragma once


using namespace System;
using namespace System::IO;
using namespace System::Collections::Generic;
using namespace System::Reflection;
using namespace System::Runtime::InteropServices;

namespace NXDO {
	namespace JBridge{
		
		public ref class JJarLoader
		{
		private:

			#pragma region ������ȡ����ķ���
			static Dictionary<ValueTypeFlag, IntPtr>^ dicV_AddParamMethods; //��Ӳ���ֵ�ķ��� addParamValue(...)
			static Dictionary<ValueTypeFlag, IntPtr>^ dicL_AddParamMethods; //��Ӳ���ֵ�ķ��� addParamValue(...)
			static Dictionary<ValueTypeFlag, IntPtr>^ dic_invokeGetMethods; //ִ�з�������ȡ����ֵ invoke[Boolean/Long/.../Object]Method
			static Dictionary<ValueTypeFlag, IntPtr>^ dic_invokeGetFields;	//��ȡ����ֵ invoke[Boolean/Long/.../Object]Field
			
			/// <summary>
			/// ��� ֵ����(������) �ķ���
			/// </summary>
			static void addToVMap(ValueTypeFlag vtFlag, jmethodID methodID){
				if(methodID == NULL)return;
				if(dicV_AddParamMethods == nullptr)
					dicV_AddParamMethods = gcnew Dictionary<ValueTypeFlag, IntPtr>();

				if(dicV_AddParamMethods->ContainsKey(vtFlag))return;
				dicV_AddParamMethods->Add(vtFlag,IntPtr(methodID));
			}

			/// <summary>
			/// ��� ��������(java.lang.Integer/...) �ķ���
			/// </summary>
			static void addToLMap(ValueTypeFlag vtFlag, jmethodID methodID){
				if(methodID == NULL)return;
				if(dicL_AddParamMethods == nullptr)
					dicL_AddParamMethods = gcnew Dictionary<ValueTypeFlag, IntPtr>();

				if(dicL_AddParamMethods->ContainsKey(vtFlag))return;
				dicL_AddParamMethods->Add(vtFlag,IntPtr(methodID));
			}

			/// <summary>
			/// ��� java�� jarLoader �в�ͬ���� invoke[Boolean/Long/.../Object]Method �ķ���
			/// </summary>
			static void addToInvokeMap(ValueTypeFlag vtFlag, jmethodID methodID){
				if(methodID == NULL)return;
				if(dic_invokeGetMethods == nullptr)
					dic_invokeGetMethods = gcnew Dictionary<ValueTypeFlag, IntPtr>();

				if(dic_invokeGetMethods->ContainsKey(vtFlag))return;
				dic_invokeGetMethods->Add(vtFlag,IntPtr(methodID));
			}

			/// <summary>
			/// ��� java�� jarLoader �в�ͬ���� invoke[Boolean/Long/.../Object]Field �ķ���
			/// </summary>
			static void addToFieldMap(ValueTypeFlag vtFlag, jmethodID methodID){
				if(methodID == NULL)return;
				if(dic_invokeGetFields == nullptr)
					dic_invokeGetFields = gcnew Dictionary<ValueTypeFlag, IntPtr>();

				if(dic_invokeGetFields->ContainsKey(vtFlag))return;
				dic_invokeGetFields->Add(vtFlag,IntPtr(methodID));
			}

			/// <summary>
			/// �ӻ����л�ȡ ֵ����(������) �ķ���
			/// </summary>
			static jmethodID getFromVMap(ValueTypeFlag vtFlag){
				if(dicV_AddParamMethods == nullptr)return NULL;
				if(!dicV_AddParamMethods->ContainsKey(vtFlag))return NULL;
				IntPtr ptr = dicV_AddParamMethods[vtFlag];
				return (jmethodID)ptr.ToPointer();
			}

			/// <summary>
			/// �ӻ����л�ȡ ��������(java.lang.Integer/...) �ķ���
			/// </summary>
			static jmethodID getFromLMap(ValueTypeFlag vtFlag){
				if(dicL_AddParamMethods == nullptr)return NULL;
				if(!dicL_AddParamMethods->ContainsKey(vtFlag))return NULL;
				IntPtr ptr = dicL_AddParamMethods[vtFlag];
				return (jmethodID)ptr.ToPointer();
			}

			/// <summary>
			/// �ӻ����л�ȡ jarLoader �в�ͬ���� invoke[Boolean/Long/.../Object]Method �ķ���
			/// </summary>
			static jmethodID getFromInvokeMap(ValueTypeFlag vtFlag){
				if(dic_invokeGetMethods == nullptr)return NULL;
				if(!dic_invokeGetMethods->ContainsKey(vtFlag))return NULL;
				IntPtr ptr = dic_invokeGetMethods[vtFlag];
				return (jmethodID)ptr.ToPointer();
			}

			/// <summary>
			/// �ӻ����л�ȡ jarLoader �в�ͬ���� invoke[Boolean/Long/.../Object]Field �ķ���
			/// </summary>
			static jmethodID getFromFieldMap(ValueTypeFlag vtFlag){
				if(dic_invokeGetFields == nullptr)return NULL;
				if(!dic_invokeGetFields->ContainsKey(vtFlag))return NULL;
				IntPtr ptr = dic_invokeGetFields[vtFlag];
				return (jmethodID)ptr.ToPointer();
			}

			static jmethodID jThrowableGetMessageMethod; //��ȡ�쳣�ķ�������Throwable.getMessage��
			static jmethodID jJarClearParamValueMethod;  //��������JarLoader.clearParamValue��
			static jmethodID jJarGetClassMethod; //JarLoader.getClass
			static jmethodID jJarNewInstanceByNameMethod; // JarLoader.newInstance(string)
			static jmethodID jJarNewInstanceByClassMethod; // JarLoader.newInstance(class)
			static jmethodID jJarInvokeObjectMethod; // JarLoader.invokeObjectMethod()
			#pragma endregion

		internal:
			jclass JarLoaderClass;
			jobject JarLoaderObject;

			JJarLoader()
			{
				//http://www.20864.com/201221/229.html,�������ձ�
				//http://book.51cto.com/art/201305/395883.htm,�������ձ�
				//javap -s -p JarLoader,�鿴����ǩ�� 
				//javassist byte[] for class

				//��ʼ��,���� java �����ļ���
				if(dicV_AddParamMethods == nullptr)
					dicV_AddParamMethods = gcnew Dictionary<ValueTypeFlag, IntPtr>();

				if(dicL_AddParamMethods == nullptr)
					dicL_AddParamMethods = gcnew Dictionary<ValueTypeFlag, IntPtr>();

				if(dic_invokeGetMethods == nullptr)
					dic_invokeGetMethods = gcnew Dictionary<ValueTypeFlag, IntPtr>();

				if(dic_invokeGetFields == nullptr)
					dic_invokeGetFields = gcnew Dictionary<ValueTypeFlag, IntPtr>();
			}

			//����jarLoader����������ʵ��
			void setJarLoader(jclass jarLoaderClass, jobject jarLoaderObject)
			{
				//jarLoaderObject,Ϊȫ������
				this->JarLoaderClass = jarLoaderClass;
				this->JarLoaderObject = jarLoaderObject;
			}

			#pragma region ��Ӳ���ֵ
			/// <summary>
			/// ��Ӳ���ֵ
			/// </summary>
			/// <param name="obj">����ֵ</param>
			/// <param name="isLangObject">�����Ƿ�Ϊ��������</param>
			/// <param name="refJavaClassName">����Ϊ��������ʱ,������java������,��֮Ϊ��</param>
			/// <param name="refJavaArrayElemClassName">����Ϊ��������ʱ,����Ԫ�ز�����java������,��֮Ϊ��</param>
			[System::ObsoleteAttribute("����,����֧��.", true)]
			void addParamValue(Object^ obj, bool isLangObject, bool isArray, String^ refJavaClassName, String^ refJavaArrayElemClassName)
			{
				//Ϊ�գ���ʾҪ���ݿ�ֵ��java��(����Ϊ��������)
				bool objIsNullValue = obj == nullptr;
				if(objIsNullValue){
					if(String::IsNullOrWhiteSpace(refJavaClassName))
						throw gcnew ArgumentNullException("refJavaClassName");

					obj = gcnew Object();
				}


				jobject joVal = NULL;
				jmethodID jAddParams = NULL;
				ValueTypeFlag vtFlag;
				const char* sign = "";

				#pragma region �������������
				if(!isArray)
				{
					if(Type::GetTypeCode(obj->GetType()) == TypeCode::Boolean)
					{
						#pragma region boolean
						vtFlag = ValueTypeFlag::VF_bool;
						bool val = (bool)obj;
						joVal = isLangObject ? lconvert::boolean2J(val) : (jobject)val;
						sign = isLangObject ?  "(Ljava/lang/Boolean;)V" : "(Z)V";
						#pragma endregion
					}
					else if(Type::GetTypeCode(obj->GetType()) == TypeCode::Byte)
					{
						#pragma region byte
						//byte
						vtFlag = ValueTypeFlag::VF_byte;
						byte val = (byte)obj;
						joVal = isLangObject ? lconvert::byte2J(val) : (jobject)val;
						sign = isLangObject ?  "(Ljava/lang/Byte;)V" : "(B)V";
						#pragma endregion
					}
					else if(Type::GetTypeCode(obj->GetType()) == TypeCode::Char)
					{
						#pragma region char
						vtFlag = ValueTypeFlag::VF_char;
						char val = (char)obj;
						joVal = isLangObject ? lconvert::char2J(val) : (jobject)val;
						sign = isLangObject ?  "(Ljava/lang/Character;)V" : "(C)V";
						#pragma endregion
					}
					else if(Type::GetTypeCode(obj->GetType()) == TypeCode::Int16)
					{
						#pragma region short
						vtFlag = ValueTypeFlag::VF_short;
						short val = (short)obj;
						joVal = isLangObject ? lconvert::short2J(val) : (jobject)val;
						sign = isLangObject ?  "(Ljava/lang/Short;)V" : "(S)V";
						#pragma endregion
					}
					else if(Type::GetTypeCode(obj->GetType()) == TypeCode::Int32)
					{
						#pragma region int
						vtFlag = ValueTypeFlag::VF_int;
						int val = (int)obj;
						joVal = isLangObject ? lconvert::integer2J(val) : (jobject)val;
						sign = isLangObject ?  "(Ljava/lang/Integer;)V" : "(I)V";
						#pragma endregion
					}
					else if(Type::GetTypeCode(obj->GetType()) == TypeCode::Int64)
					{
						#pragma region long
						vtFlag = ValueTypeFlag::VF_long;
						long val = (long)obj;
						joVal = isLangObject ? lconvert::long2J(val) : (jobject)val;
						sign = isLangObject ?  "(Ljava/lang/Long;)V" : "(J)V";
						#pragma endregion
					}
					else if(Type::GetTypeCode(obj->GetType()) == TypeCode::Single)
					{
						#pragma region float (ֵ����,���ú󷵻�)
						vtFlag = ValueTypeFlag::VF_float;
						float val = (float)obj;
						if(isLangObject){
							joVal = lconvert::float2J(val);
							sign = "(Ljava/lang/Float;)V";						
						}
						else{
							jfloat jp = val;
							jAddParams = getFromVMap(ValueTypeFlag::VF_float);
							if(jAddParams == NULL)
							{
								jAddParams = jvmrun::getEnv()->GetMethodID(JarLoaderClass, "addParamValue","(F)V");
								addToVMap(ValueTypeFlag::VF_float,jAddParams);
							}
							jvmrun::getEnv()->CallVoidMethod(JarLoaderObject, jAddParams, jp);
							this->HasJavaExceptionByThrow();
							return;
						}
						#pragma endregion
					}
					else if(Type::GetTypeCode(obj->GetType()) == TypeCode::Double)
					{
						#pragma region double (ֵ����,���ú󷵻�)
						vtFlag = ValueTypeFlag::VF_double;
						double val = (double)obj;
						if(isLangObject){
							joVal = lconvert::double2J(val);
							sign = "(Ljava/lang/Double;)V";
						}
						else{
							jdouble jp = (jdouble)val;
							jAddParams = getFromVMap(ValueTypeFlag::VF_double);
							if(jAddParams == NULL)
							{
								jAddParams = jvmrun::getEnv()->GetMethodID(JarLoaderClass, "addParamValue","(D)V");
								addToVMap(ValueTypeFlag::VF_double, jAddParams);
							}
							jvmrun::getEnv()->CallVoidMethod(JarLoaderObject, jAddParams, jp);
							this->HasJavaExceptionByThrow();
							return;
						}
						#pragma endregion
					}
					else if(Type::GetTypeCode(obj->GetType()) == TypeCode::String)
					{
						#pragma region string (���ú󷵻�)
						jstring jstr = getJString((String^)obj);

						jAddParams = getFromLMap(ValueTypeFlag::VF_string);
						if(jAddParams == NULL)
						{
							jAddParams = jvmrun::getEnv()->GetMethodID(JarLoaderClass, "addParamValue","(Ljava/lang/String;)V");
							addToLMap(ValueTypeFlag::VF_string, jAddParams);
						}
						jvmrun::getEnv()->CallVoidMethod(JarLoaderObject, jAddParams, jstr);
						this->HasJavaExceptionByThrow();
						return;
						#pragma endregion
					}
					else if(Type::GetTypeCode(obj->GetType()) == TypeCode::Object)
					{
						#pragma region object (���ú󷵻�,������������������������ֻ��һ������)
						vtFlag = ValueTypeFlag::VF_object;

						//addParamValue(null, className) ֧�ֿ�,��ע���������д���
						//if(ijobj == nullptr)
						//	throw gcnew NullReferenceException("JarLoader δ�������������ƥ��� addParamValue ������");
					
						jAddParams = getFromLMap(ValueTypeFlag::VF_object);
						if(jAddParams == NULL)
						{
							jAddParams = jvmrun::getEnv()->GetMethodID(JarLoaderClass, "addParamValue","(Ljava/lang/Object;Ljava/lang/String;)V");
							addToLMap(ValueTypeFlag::VF_object, jAddParams);
						}

						jobject joValue = objIsNullValue ? NULL : (jobject)((IObject^)obj)->JObjectPtr.ToPointer();
						jstring jObjClassName = this->getJString(refJavaClassName);					
						jvmrun::getEnv()->CallVoidMethod(JarLoaderObject, jAddParams, joValue, jObjClassName);
						this->HasJavaExceptionByThrow();
						return;
						#pragma endregion
					}
					else
						throw gcnew NullReferenceException("JarLoader δ�������������ƥ��� addParamValue ������");


					jAddParams = isLangObject ? getFromLMap(vtFlag) : getFromVMap(vtFlag);
					if(jAddParams == NULL)
					{
						jAddParams = jvmrun::getEnv()->GetMethodID(JarLoaderClass, "addParamValue", sign);
						if(isLangObject)addToLMap(vtFlag, jAddParams);
						else addToVMap(vtFlag, jAddParams);
					}

					if(jAddParams == NULL)
						throw gcnew NullReferenceException("JarLoader������ addParamValue( "+ gcnew String(sign) +" )������");

					jvmrun::getEnv()->CallVoidMethod(JarLoaderObject, jAddParams, joVal);
					this->HasJavaExceptionByThrow();
					return;
				}
				#pragma endregion

				jAddParams = getFromLMap(ValueTypeFlag::VF_object);
				if(jAddParams == NULL)
				{
					jAddParams = jvmrun::getEnv()->GetMethodID(JarLoaderClass, "addParamValue","(Ljava/lang/Object;Ljava/lang/String;)V");					
					addToLMap(ValueTypeFlag::VF_object, jAddParams);
				}

				char* elemClassName = (char*)(Marshal::StringToHGlobalAnsi(refJavaArrayElemClassName)).ToPointer();
				array<Object^,1>^ objAry = (array<Object^,1>^)obj;
				int size = objAry == nullptr ? 0 : objAry->Length;
				jclass aryElemClass = jvmrun::getEnv()->FindClass(elemClassName);
				jobjectArray texts= jvmrun::getEnv()->NewObjectArray(size, aryElemClass, 0);
				for(int i=0; i<size; i++)
				{
					String^ strAryVal = (String^)objAry[i];
					jstring jstr = this->getJString(strAryVal);
					jvmrun::getEnv()->SetObjectArrayElement(texts,i,jstr);
				}
				jvmrun::getEnv()->CallVoidMethod(JarLoaderObject, jAddParams, texts, this->getJString(refJavaClassName));
				//jvmrun::getEnv()->ReleaseBooleanArrayElements
				Marshal::FreeHGlobal(IntPtr((void*)elemClassName));

				this->HasJavaExceptionByThrow();
				return;



			}
			

			void addParamValueToJava(Object^ obj, bool isSubOfJObject, bool isLangObject,  ValueTypeFlag typeFlag, String^ refJavaClassName)
			{
				const char* sign = "";
				jobject joVal = NULL;
				jfloat jFloatValue;
				jdouble jDoubleValue;
				jmethodID jAddParams = NULL;


				jstring jObjClassName = this->getJString(refJavaClassName);	
				if(obj == nullptr)
				{
					#pragma region �������Ϊ�գ�ʹ���������͵ķ������ݲ����������ø����������͡�
					jAddParams = getFromLMap(ValueTypeFlag::VF_object);
					if(jAddParams == NULL){
						jAddParams = jvmrun::getEnv()->GetMethodID(JarLoaderClass, "addParamValue","(Ljava/lang/Object;Ljava/lang/String;)V");
						addToLMap(ValueTypeFlag::VF_object, jAddParams);
					}

					jvmrun::getEnv()->CallVoidMethod(JarLoaderObject, jAddParams, joVal, jObjClassName);
					this->HasJavaExceptionByThrow();
					return;
					#pragma endregion
				}

				#pragma region ����������
				if(typeFlag == ValueTypeFlag::VF_bool)
				{
					joVal = isLangObject ? lconvert::boolean2J((bool)obj) : (jobject)((bool)obj);
					sign = isLangObject ? "" : "(Z)V";
				}

				if(typeFlag == ValueTypeFlag::VF_byte)
				{
					joVal = isLangObject ? lconvert::byte2J((byte)obj) : (jobject)((byte)obj);
					sign = isLangObject ? "" :"(B)V";
				}

				if(typeFlag == ValueTypeFlag::VF_char){
					joVal = isLangObject ? lconvert::char2J((Char)obj) : (jobject)((Char)obj);
					sign = isLangObject ? "" :"(C)V";
				}

				if(typeFlag == ValueTypeFlag::VF_short){
					joVal = isLangObject ? lconvert::short2J((short)obj) : (jobject)((short)obj);
					sign = isLangObject ? "" :"(S)V";
				}

				if(typeFlag == ValueTypeFlag::VF_int){
					joVal = isLangObject ? lconvert::integer2J((int)obj) : (jobject)((int)obj);
					sign = isLangObject ? "" :"(I)V";
				}

				if(typeFlag == ValueTypeFlag::VF_long){
					joVal = isLangObject ? lconvert::long2J((Int64)obj) : (jobject)((Int64)obj);
					sign = isLangObject ? "" :"(J)V";
				}

				if(typeFlag == ValueTypeFlag::VF_float)
				{
					if(isLangObject)
						joVal = lconvert::float2J((float)obj);
					else{
						jFloatValue = (float)obj;
						sign = "(F)V";
					}
				}

				if(typeFlag == ValueTypeFlag::VF_double)
				{
					if(isLangObject)
						joVal = lconvert::double2J((double)obj);
					else{
						jDoubleValue = (double)obj;
						sign = "(D)V";
					}
				}

				if(typeFlag == ValueTypeFlag::VF_string)
					joVal = getJString((String^)obj);

				if(typeFlag == ValueTypeFlag::VF_object){
					if(isSubOfJObject)
						joVal = (jobject)((IObject^)obj)->JObjectPtr.ToPointer();
					else
					{
						if(obj->GetType() == Boolean::typeid)
							joVal = lconvert::boolean2J((bool)obj);
						else if(obj->GetType() == Byte::typeid)
							joVal = lconvert::byte2J((Byte)obj);
						else if(obj->GetType() == Char::typeid)
							joVal = lconvert::char2J((Char)obj);
						else if(obj->GetType() == Int16::typeid)
							joVal = lconvert::short2J((short)obj);
						else if(obj->GetType() == Int32::typeid)
							joVal = lconvert::integer2J((int)obj);
						else if(obj->GetType() == Int64::typeid)
							joVal = lconvert::long2J((Int64)obj);
						else if(obj->GetType() == Single::typeid)
							joVal = lconvert::float2J((float)obj);
						else if(obj->GetType() == Double::typeid)
							joVal = lconvert::double2J((double)obj);
						else if(obj->GetType() == String::typeid)
							joVal = getJString((String^)obj);
						else {
							IObject^ IObj = (IObject^)obj;
							if(IObj != nullptr)
								joVal = (jobject)(IObj->JObjectPtr.ToPointer());
						}
						
					}
				}

				#pragma endregion

				//ǩ���ǿգ���Ϊֵ���ʹ���
				if(sign != "") 
				{
					//����ֵ���͵Ĳ���ֵ
					jAddParams = getFromVMap(typeFlag);
					if(jAddParams == NULL){
						jAddParams = jvmrun::getEnv()->GetMethodID(JarLoaderClass, "addParamValue", sign);
						addToVMap(typeFlag, jAddParams);
					}

					//float,double ��ֵ���ͣ��޷�ת�� jobject ���ݸ� java, �жϺ���� java ������
					if(typeFlag == ValueTypeFlag::VF_float)
						jvmrun::getEnv()->CallVoidMethod(JarLoaderObject, jAddParams, jFloatValue);
					else if(typeFlag == ValueTypeFlag::VF_float)
						jvmrun::getEnv()->CallVoidMethod(JarLoaderObject, jAddParams, jDoubleValue);
					else
						jvmrun::getEnv()->CallVoidMethod(JarLoaderObject, jAddParams, joVal);
					this->HasJavaExceptionByThrow();
				}
				else
				{
					//�������͵Ĳ���ֵ
					jAddParams = getFromLMap(ValueTypeFlag::VF_object);
					if(jAddParams == NULL){
						jAddParams = jvmrun::getEnv()->GetMethodID(JarLoaderClass, "addParamValue","(Ljava/lang/Object;Ljava/lang/String;)V");
						addToLMap(ValueTypeFlag::VF_object, jAddParams);
					}

					jvmrun::getEnv()->CallVoidMethod(JarLoaderObject, jAddParams, joVal, jObjClassName);
					this->HasJavaExceptionByThrow();
				}
			}
			
			/// <summary>
			/// ��Ӳ���ֵ
			/// </summary>
			/// <param name="obj">����ֵ��ֵ�������飬ֵ�������������飬String[], ����JObject[]����</param>
			/// <param name="isSubOfJObject">����Ԫ���Ƿ�Ϊ JObject �����ࣩ</param>
			/// <param name="isLangObject">����Ԫ���Ƿ�Ϊ��������</param>
			/// <param name="typeFlag">���ͱ�ʾ</param>
			/// <param name="refJavaClassName">java���͵��ַ�����ʽ��[Lxxx;</param>
			/// <param name="refJavaArrayElemClassName">����Ԫ��java���͵��ַ�����ʽ��java/lang/xxx</param>
			void addParamArrayToJava(Object^ obj, bool isSubOfJObject, bool isLangObject,  ValueTypeFlag typeFlag, String^ refJavaClassName, String^ refJavaArrayElemClassName)
			{
				//������������ƣ����ݵ� java �㣬javaʹ�� [.] �ָ����͡� [Ljava.lang.Integer [I
				jstring jObjClassName = this->getJString(refJavaClassName);	

				jobject joVal = NULL;
				jmethodID jAddParams = NULL;
				jobjectArray jobjAry = NULL;

				System::Array^ objAry = (System::Array^)obj;

				//larray::toJObjectArray(objAry);
				//larray::toJShortArray(objAry);
				//larray::toJIntArray(objAry);

				//array<Object^,1>^ objAry = (array<Object^,1>^)obj;
				int size = objAry == nullptr ? 0 : objAry->Length;

				#pragma region ���� java ���������
				if(!isLangObject)
				{
					//ֵ�����������
					if(typeFlag == ValueTypeFlag::VF_bool)
						jobjAry = (jobjectArray)jvmrun::getEnv()->NewBooleanArray(size);

					if(typeFlag == ValueTypeFlag::VF_byte)
						jobjAry = (jobjectArray)jvmrun::getEnv()->NewByteArray(size);

					if(typeFlag == ValueTypeFlag::VF_char)
						jobjAry = (jobjectArray)jvmrun::getEnv()->NewCharArray(size);

					if(typeFlag == ValueTypeFlag::VF_short)
						jobjAry = (jobjectArray)jvmrun::getEnv()->NewShortArray(size);

					if(typeFlag == ValueTypeFlag::VF_int)
						jobjAry = (jobjectArray)jvmrun::getEnv()->NewIntArray(size);

					if(typeFlag == ValueTypeFlag::VF_long)
						jobjAry = (jobjectArray)jvmrun::getEnv()->NewLongArray(size);

					if(typeFlag == ValueTypeFlag::VF_float)
						jobjAry = (jobjectArray)jvmrun::getEnv()->NewFloatArray(size);

					if(typeFlag == ValueTypeFlag::VF_double)
						jobjAry = (jobjectArray)jvmrun::getEnv()->NewDoubleArray(size);
				}
				else
				{
					//string,object,integer... ���������������
					jclass aryElemClass = NULL;
					if(!isSubOfJObject)
					{
						char* elemClassName = (char*)(Marshal::StringToHGlobalAnsi(refJavaArrayElemClassName)).ToPointer();
						aryElemClass = jvmrun::getEnv()->FindClass(elemClassName);
						Marshal::FreeHGlobal(IntPtr((void*)elemClassName));
						if(aryElemClass == NULL)
							this->HasJavaExceptionByThrow();
					}
					else{
						//��Ϊ���ݵ� java ����ã�����ʹ�� [.] �ָ�
						String^ refElemClassName = refJavaArrayElemClassName;
						if(refElemClassName->IndexOf("/") > -1)
							refElemClassName = refElemClassName->Replace("/",".");

						IntPtr intAryElemPtr = this->GetClass(refElemClassName, nullptr);
						aryElemClass = (jclass)intAryElemPtr.ToPointer();

						if(aryElemClass == NULL)
							this->HasJavaExceptionByThrow();
					}
					jobjAry = jvmrun::getEnv()->NewObjectArray(size, aryElemClass, 0);	
				}
				#pragma endregion

				#pragma region �ֽ��������Ԫ�ص�ֵ����ֵ�� java ��������				
				for(int i=0; i<size; i++)
				{
					Object^ oElemValue = objAry->GetValue(i);
					jobject jElemValue = nullptr;

					if(oElemValue != nullptr)
					{
						if(typeFlag == ValueTypeFlag::VF_bool){
							if(isLangObject)
								jElemValue = lconvert::boolean2J((bool)oElemValue) ;
							else{
								jboolean bval = (bool)oElemValue;
								jvmrun::getEnv()->SetBooleanArrayRegion((jbooleanArray)jobjAry, i, 1, &bval);
								this->HasJavaExceptionByThrow();
								continue;
							}
						}

						if(typeFlag == ValueTypeFlag::VF_byte){
							if(isLangObject)
								jElemValue = lconvert::byte2J((byte)oElemValue);
							else{
								jbyte bval = (byte)oElemValue;
								jvmrun::getEnv()->SetByteArrayRegion((jbyteArray)jobjAry, i, 1, &bval);
								this->HasJavaExceptionByThrow();
								continue;
							}
						}

						if(typeFlag == ValueTypeFlag::VF_char){
							if(isLangObject)
								jElemValue = lconvert::char2J((Char)oElemValue);
							else{
								jchar cval = Convert::ToChar(oElemValue);
								jvmrun::getEnv()->SetCharArrayRegion((jcharArray)jobjAry, i, 1, &cval);
								this->HasJavaExceptionByThrow();
								continue;
							}
						}

						if(typeFlag == ValueTypeFlag::VF_short){
							if(isLangObject)
								jElemValue = lconvert::short2J((short)oElemValue);
							else{
								jshort sval = (short)oElemValue;
								jvmrun::getEnv()->SetShortArrayRegion((jshortArray)jobjAry, i, 1, &sval);
								this->HasJavaExceptionByThrow();
								continue;
							}
						}

						if(typeFlag == ValueTypeFlag::VF_int){
							if(isLangObject)
								jElemValue = lconvert::integer2J((int)oElemValue);
							else{
								jint ival = (int)oElemValue;
								jvmrun::getEnv()->SetIntArrayRegion((jintArray)jobjAry, i, 1, &ival);
								this->HasJavaExceptionByThrow();
								continue;
							}
						}

						if(typeFlag == ValueTypeFlag::VF_long){
							if(isLangObject)
								jElemValue = lconvert::long2J((Int64)oElemValue);
							else{
								jlong lval = Convert::ToInt64(oElemValue);
								jvmrun::getEnv()->SetLongArrayRegion((jlongArray)jobjAry, i, 1, &lval);
								this->HasJavaExceptionByThrow();
								continue;
							}
						}

						if(typeFlag == ValueTypeFlag::VF_float)
						{
							if(isLangObject)
								jElemValue = lconvert::float2J((float)oElemValue);
							else{
								jfloat fval = (float)oElemValue;
								jvmrun::getEnv()->SetFloatArrayRegion((jfloatArray)jobjAry,i, 1, &fval);
								this->HasJavaExceptionByThrow();
								continue;
							}
						}

						if(typeFlag == ValueTypeFlag::VF_double)
						{
							if(isLangObject)
								jElemValue = lconvert::double2J((double)oElemValue);
							else{
								jdouble dval = (double)oElemValue;
								jvmrun::getEnv()->SetDoubleArrayRegion((jdoubleArray)jobjAry,i, 1, &dval);
								this->HasJavaExceptionByThrow();
								continue;
							}
						}

						if(typeFlag == ValueTypeFlag::VF_string)
							jElemValue = this->getJString((String^)oElemValue);

						if(typeFlag == ValueTypeFlag::VF_object){

							IObject^ jObjectInstance = nullptr;
							try
							{
								jObjectInstance = (IObject^)oElemValue;
							}catch(...){}

							if(jObjectInstance != nullptr){
								IntPtr jObjElemPtr = jObjectInstance->JObjectPtr;
								jElemValue = (jobject)jObjElemPtr.ToPointer();
							}
							else{
								//���Ͳ�������Ԫ��ֵ��typeFlag��ΪVF_object
								if(oElemValue->GetType() == System::Boolean::typeid) 
									jElemValue = lconvert::boolean2J((bool)oElemValue) ;
								else if(oElemValue->GetType() == System::Byte::typeid) 
									jElemValue = lconvert::byte2J((System::Byte)oElemValue);
								else if(oElemValue->GetType() == System::Char::typeid) 
									jElemValue = lconvert::char2J((Char)oElemValue);
								else if(oElemValue->GetType() == Int16::typeid) 
									jElemValue = lconvert::short2J((short)oElemValue);
								else if(oElemValue->GetType() == Int32::typeid) 
									jElemValue = lconvert::integer2J((int)oElemValue);
								else if(oElemValue->GetType() == Int64::typeid) 
									jElemValue = lconvert::long2J((Int64)oElemValue);
								else if(oElemValue->GetType() == System::Single::typeid) 
									jElemValue = lconvert::float2J((float)oElemValue);
								else if(oElemValue->GetType() == System::Double::typeid) 
									jElemValue = lconvert::double2J((double)oElemValue);
								else if(oElemValue->GetType() == String::typeid) 
									jElemValue = this->getJString((String^)oElemValue);
							}
						}
					}

					jvmrun::getEnv()->SetObjectArrayElement(jobjAry,i, jElemValue);
					this->HasJavaExceptionByThrow();
				}

				joVal = jobjAry;
				#pragma endregion

				jAddParams = getFromLMap(ValueTypeFlag::VF_object);
				if(jAddParams == NULL){
					jAddParams = jvmrun::getEnv()->GetMethodID(JarLoaderClass, "addParamValue","(Ljava/lang/Object;Ljava/lang/String;)V");
					addToLMap(ValueTypeFlag::VF_object, jAddParams);
				}

				jvmrun::getEnv()->CallVoidMethod(JarLoaderObject, jAddParams, joVal, jObjClassName);
				this->HasJavaExceptionByThrow();
			}
			#pragma endregion

			#pragma region �й�String ת�� jstring 
			/// <summary>
			/// �й�String ת���� jstring 
			/// </summary>
			/// <return>jstringʵ��<return>
			jstring getJString(String^ strVal)
			{
				char* tmpval = (char*)(Marshal::StringToHGlobalAnsi(strVal)).ToPointer();
				jstring jStr= lconvert::string2J(tmpval);
				Marshal::FreeHGlobal(IntPtr((void*)tmpval));

				return jStr;
			}
			#pragma endregion

			#pragma region java�쳣����			
			/// <summary>
			/// ����java����ʱ�쳣,���׳��쳣.
			/// </summary>
			void HasJavaExceptionByThrow(){
				if(jvmrun::getEnv()->ExceptionCheck() != JNI_TRUE)return;

				jthrowable jthexp = jvmrun::getEnv()->ExceptionOccurred();
				
				if(jThrowableGetMessageMethod == NULL){
					jclass jClassExp = jvmrun::getEnv()->FindClass("java/lang/Throwable");
					jThrowableGetMessageMethod = jvmrun::getEnv()->GetMethodID(jClassExp, "getMessage", "()Ljava/lang/String;");
				}
				jstring jstrMsg =  (jstring)jvmrun::getEnv()->CallObjectMethod(jthexp, jThrowableGetMessageMethod);
				
				jvmrun::getEnv()->ExceptionDescribe();
				jvmrun::getEnv()->ExceptionClear();
				
				Exception^ exp = nullptr;
				if(jstrMsg != NULL){
					String^ strMsg = "java exception��" + gcnew String(lconvert::j2string(jstrMsg)) + "��";
					exp = gcnew MethodAccessException(strMsg);
				}
				else
					exp = gcnew MethodAccessException("java exception�����쳣�׳����޾�����Ϣ��");
				throw exp;
			}
			#pragma endregion

		public:

			/// <summary>
			/// ����ϴε���ǰ�����õĲ����б�
			/// </summary>
			void ClearParams()
			{
				if(jJarClearParamValueMethod == NULL)
					jJarClearParamValueMethod = jvmrun::getEnv()->GetMethodID(JarLoaderClass, "clearParamValue","()V");
				jvmrun::getEnv()->CallObjectMethod(JarLoaderObject, jJarClearParamValueMethod);
				this->HasJavaExceptionByThrow();
			}


			///// <summary>
			///// ����ǰ���÷����Ĳ����б�.
			///// </summary>
			///// <param name="val">����ֵ</param>
			///// <param name="isLangObject">�Ƿ����ڶ�������.ֵ����Ϊfalse.</param>
			///// <param name="refJavaClassNames">����Ϊjava.lang.Object����ʱ,������java������,��֮Ϊ�ա�ֵ���Ͷ�Ӧ���������Ͳ�ʹ�ô˲�����</param>
			//void FillParamValue(Object^ val, bool isLangObject, bool isArray, String^ refJavaClassName, String^ refJavaArrayElemClassName)
			//{
			//	//refJavaClassNames
			//	//����Ϊ��������ʱ,������java������,��֮Ϊ�� [�˲���Ϊ���� JarLoader.addParamValue(Object,String)]

			//	/*String^ refJavaClassName = nullptr;
			//	if(refJavaClassNames != nullptr)
			//	{
			//		if(refJavaClassNames->Length > 0)
			//			refJavaClassName = refJavaClassNames[0];
			//	}*/

			//	String^ refElemClassName = refJavaArrayElemClassName;
			//	if(!String::IsNullOrWhiteSpace(refJavaArrayElemClassName))
			//		refElemClassName = refJavaArrayElemClassName->Replace(".","/");
			//	
			//	this->addParamValue(val,isLangObject, isArray, refJavaClassName, refElemClassName);

			//	//test				
			//}

			/// <summary>
			/// ��������ǰ�����÷����Ĳ����б�ֵ��
			/// </summary>
			/// <param name="obj">��ͨ����ֵ������</param>
			/// <param name="isSubOfJObject">����ֵΪ���飬����Ч������Ԫ���Ƿ�Ϊ JObject �����ࣩ</param>
			/// <param name="isLangObject">��ͨ����������Ԫ�أ��Ƿ�Ϊ�������͡�</param>
			/// <param name="typeFlag">��ͨ����������Ԫ�ص����ͱ�ʾ</param>
			/// <param name="refJavaClassName">��ͨ����������Ԫ�ص� java �����ַ�����ʽ��</param>
			/// <param name="refJavaArrayElemClassName">��ͨ����ʱΪ�ա�����Ԫ��java���͵��ַ�����ʽ��java/lang/xxx</param>
			void FillJavaParamValues(Object^ obj, bool isSubOfJObject, bool isLangObject,  bool isArray, ValueTypeFlag typeFlag, String^ refJavaClassName, String^ refJavaArrayElemClassName)
			{
				String^ refElemClassName = refJavaArrayElemClassName;
				if(!String::IsNullOrWhiteSpace(refJavaArrayElemClassName))
					refElemClassName = refJavaArrayElemClassName->Replace(".","/"); //��Ҫ��JNI�л�ȡCLASS,����ת��,��refJavaClassName,ֱ����JAVA��ת��.

				if(!isArray)
					this->addParamValueToJava(obj, isSubOfJObject, isLangObject, typeFlag, refJavaClassName);
				else
					this->addParamArrayToJava(obj, isSubOfJObject, isLangObject, typeFlag, refJavaClassName, refElemClassName);
			}

			/*void FillParamValue(... array<Object^,1>^ values)
			{
				int iCount = values->Length;
				for(int i=0; i<iCount;i++){
					Object^ mo = values->GetValue(i);

					this->addParamValue(mo,false);
				}
			}*/

			IntPtr GetClass(String^ className, Object^ IJObjectface)
			{				
				if(String::IsNullOrWhiteSpace(className))
					throw gcnew System::ArgumentNullException("className","����ָ�� java ���������ơ�");

				jstring jclassName = getJString(className);

				if(jJarGetClassMethod == NULL)
					jJarGetClassMethod = jvmrun::getEnv()->GetMethodID(JarLoaderClass, "getClass","(Ljava/lang/String;)Ljava/lang/Class;");
				jclass jcls = (jclass)jvmrun::getEnv()->CallObjectMethod(JarLoaderObject, jJarGetClassMethod, jclassName);
				this->HasJavaExceptionByThrow();
				if(jcls == NULL)return IntPtr::Zero;

				if(IJObjectface != nullptr){
					IObject^ ijobj = (IObject^)IJObjectface;
					ijobj->setJClassPtr(IntPtr(jcls));
				}

				//if(ijobj->JJarLoader == nullptr)ijobj->JJarLoader = this;
				return System::IntPtr(jcls);
			}

			IntPtr NewInstance(String^ className, Object^ IJObjectface)
			{				
				IObject^ ijobj = (IObject^)IJObjectface;
				jstring jclassName = getJString(className);

				if(jJarNewInstanceByNameMethod == NULL)
					jJarNewInstanceByNameMethod = jvmrun::getEnv()->GetMethodID(JarLoaderClass, "newInstance","(Ljava/lang/String;)Ljava/lang/Object;");
				jobject jobj = jvmrun::getEnv()->CallObjectMethod(JarLoaderObject, jJarNewInstanceByNameMethod, jclassName);
				this->HasJavaExceptionByThrow();
				if(jobj == NULL)return IntPtr::Zero;

				if(ijobj != nullptr){
					if(ijobj->JClassPtr == IntPtr::Zero){
						jclass jcls = jvmrun::getEnv()->GetObjectClass(jobj);
						ijobj->setJClassPtr(IntPtr(jcls));
					}
					ijobj->setJObjectPtr(IntPtr(jobj));
					//if(ijobj->JJarLoader == nullptr)ijobj->JJarLoader = this;
				}
				return IntPtr(jobj);
			}

			IntPtr NewInstance(IntPtr classPtr, Object^ IJObjectface)
			{				
				if(classPtr == IntPtr::Zero)
					throw gcnew NullReferenceException("��ָ�����ã��޷���ȡ��Ӧ��java���͡�");

				IObject^ ijobj = (IObject^)IJObjectface;
				
				jclass jcls = (jclass)classPtr.ToPointer();
				if(jJarNewInstanceByClassMethod == NULL)
					jJarNewInstanceByClassMethod = jvmrun::getEnv()->GetMethodID(JarLoaderClass, "newInstance","(Ljava/lang/Class;)Ljava/lang/Object;");
				jobject jobj = jvmrun::getEnv()->CallObjectMethod(JarLoaderObject, jJarNewInstanceByClassMethod, jcls);
				this->HasJavaExceptionByThrow();
				if(jobj == NULL)return IntPtr::Zero;

				if(ijobj != nullptr){
					if(ijobj->JClassPtr == IntPtr::Zero)
						ijobj->setJClassPtr(classPtr);
					ijobj->setJObjectPtr(IntPtr(jobj));
					//if(ijobj->JJarLoader == nullptr)ijobj->JJarLoader = this;
				}
				return IntPtr(jobj);
			}
			
			/// <summary>
			/// ִ��java��������ȡ���ؽ����ָ��.
			/// </summary>
			/// <param name="objectPtr">java ʵ��ָ�������ָ�� (java.lang.Object/java.lang.Class)</param>
			/// <param name="methodName">java ��������</param>
			/// <param name="isStatics">true:��̬��������֮Ϊʵ��������<para>ȱʡΪʵ��������</para></param>
			IntPtr GetObjectInvokeMethod(IntPtr objectPtr, String^ methodName, ... array<bool,1>^ isStatics)
			{
				if(objectPtr == IntPtr::Zero)
					throw gcnew NullReferenceException("��ָ�����ã��޷���ȡ��Ӧ��java���ͻ����");

				bool isStatic = false;
				int iCount = isStatics->Length;
				if(iCount == 0)
					isStatic = false;
				else
					isStatic = (bool)isStatics->GetValue(0);

				
				if(jJarInvokeObjectMethod == NULL)
					jJarInvokeObjectMethod = jvmrun::getEnv()->GetMethodID(JarLoaderClass, "invokeObjectMethod","(Ljava/lang/Object;Ljava/lang/String;)Ljava/lang/Object;");
				jstring jMethodName = getJString(methodName);
				jobject jreturn = jvmrun::getEnv()->CallObjectMethod(
						JarLoaderObject,
						jJarInvokeObjectMethod, 
						!isStatic ? (jobject)objectPtr.ToPointer() : (jclass)objectPtr.ToPointer(), 
						jMethodName);
				this->HasJavaExceptionByThrow();

				/*JNIEnv* env = jvmrun::getEnv();
				jclass j_IntegerClass = env->FindClass("Ljava/lang/Integer;");
				jmethodID m_initInteger = env->GetMethodID(j_IntegerClass, "intValue","()I");
				jint jj2 = env->CallIntMethod(jreturn, m_initInteger);
				this->HasJavaExceptionByThrow();

				jint jj = reinterpret_cast<jint>(&jreturn);
				IntPtr ptr = IntPtr((void*)(jreturn));
				int vv = ptr.ToInt32();*/

				if(jreturn == NULL)
					return IntPtr::Zero;
				return IntPtr(jreturn);
			}

			/// <summary>
			/// ִ��java��������ȡ������.
			/// <para>JObject����[]</para>
			/// </summary>
			Object^ GetObjectArrayInvokeMethod(IntPtr objectPtr, String^ methodName, Type^ NObjType, ... array<bool,1>^ isStatics)
			{
				if(objectPtr == IntPtr::Zero)
					throw gcnew NullReferenceException("��ָ�����ã��޷���ȡ��Ӧ��java���ͻ����");

				IntPtr resultPtr = GetObjectInvokeMethod(objectPtr, methodName, isStatics);
				jobject resultObj = (jobject)resultPtr.ToPointer();
				return this->ConvertJResultToArray(resultObj, true, NObjType, ValueTypeFlag::VF_object);

				#pragma region ע��
				//jobjectArray* arr = reinterpret_cast<jobjectArray*>(&resultObj);
				//int size = jvmrun::getEnv()->GetArrayLength(*arr);

				//Type^ lstGType = System::Collections::Generic::List::typeid;
				//Object^ objList = Activator::CreateInstance(lstGType->MakeGenericType(NObjType));
				//System::Collections::IList^ gList = (System::Collections::IList^)objList;

				////array<System::Object^,1>^ ndatas = gcnew array<System::Object^,1>(size);

				//for (int i = 0; i < size; i++) {
				//	jobject data = jvmrun::getEnv()->GetObjectArrayElement(*arr, i);
				//	if(data == NULL){
				//		gList->Add(nullptr);
				//		continue;
				//	}
				//	//ndatas[i] = CastToNObject(IntPtr(data), NObjType);
				//	gList->Add(CastToNObject(IntPtr(data), NObjType));
				//}

				//array<Object^>^ values = { gList };
				//MethodInfo^ toArray = (System::Linq::Enumerable::typeid)->GetMethod("ToArray")->MakeGenericMethod(NObjType);
    //            return toArray->Invoke(nullptr, values);

				//return gList;
				#pragma endregion
			}

			/// <summary>
			/// ִ��java��������ȡ������.
			/// <para>java.lang.Integer��������������</para>
			/// </summary>
			/// <param name="objectPtr">ʵ����ʵ�������������ͣ���̬��������ָ��</param>
			/// <param name="methodName">��������</param>
			Object^ GetValueNullableArrayInvokeMethod(IntPtr objectPtr, String^ methodName, ValueTypeFlag vtFlag, ... array<bool,1>^ isStatics)
			{
				if(objectPtr == IntPtr::Zero)
					throw gcnew NullReferenceException("��ָ�����ã��޷���ȡ��Ӧ��java���ͻ����");


				IntPtr resultPtr = GetObjectInvokeMethod(objectPtr, methodName, isStatics);
				jobject resultObj = (jobject)resultPtr.ToPointer();

				return this->ConvertJResultToArray(resultObj, true, nullptr, vtFlag);

				#pragma region ע��
				//jobjectArray* arr = reinterpret_cast<jobjectArray*>(&resultObj);
				//int size = jvmrun::getEnv()->GetArrayLength(*arr);
				////array<System::Object^,1>^ ndatas = gcnew array<System::Object^,1>(size);

				//#pragma region ��ȡ��Ӧ�� List<Nullable<valueType>>
				//System::Collections::IList^ gList = nullptr;
				//Type^ aryElemType = nullptr;
				//if(vtFlag == ValueTypeFlag::VF_bool){
				//	gList = gcnew System::Collections::Generic::List<Nullable<Boolean>>();
				//	aryElemType = Nullable<Boolean>::typeid;
				//}
				//else if(vtFlag == ValueTypeFlag::VF_byte){
				//	gList = gcnew System::Collections::Generic::List<Nullable<Byte>>();
				//	aryElemType = Nullable<Byte>::typeid;
				//}
				//else if(vtFlag == ValueTypeFlag::VF_char){
				//	gList = gcnew System::Collections::Generic::List<Nullable<Char>>();
				//	aryElemType = Nullable<Char>::typeid;
				//}
				//else if(vtFlag == ValueTypeFlag::VF_short){
				//	gList = gcnew System::Collections::Generic::List<Nullable<Int16>>();
				//	aryElemType = Nullable<Int16>::typeid;
				//}
				//else if(vtFlag == ValueTypeFlag::VF_int){
				//	gList = gcnew System::Collections::Generic::List<Nullable<Int32>>();
				//	aryElemType = Nullable<Int32>::typeid;
				//}
				//else if(vtFlag == ValueTypeFlag::VF_long){
				//	gList = gcnew System::Collections::Generic::List<Nullable<Int64>>();
				//	aryElemType = Nullable<Int64>::typeid;
				//}
				//else if(vtFlag == ValueTypeFlag::VF_float){
				//	gList = gcnew System::Collections::Generic::List<Nullable<Single>>();
				//	aryElemType = Nullable<Single>::typeid;
				//}
				//else if(vtFlag == ValueTypeFlag::VF_double){
				//	gList = gcnew System::Collections::Generic::List<Nullable<Double>>();
				//	aryElemType = Nullable<Double>::typeid;
				//}
				//#pragma endregion

				//for (int i = 0; i < size; i++) {
				//	jobject data = jvmrun::getEnv()->GetObjectArrayElement(*arr, i);
				//	if(data == NULL){
				//		gList->Add(nullptr);
				//		continue;
				//	}

				//	//ndatas[i] = CastToNValueNullable(IntPtr(data), vtFlag);
				//	gList->Add(CastToNValueNullable(IntPtr(data), vtFlag));
				//}

				//array<Object^>^ values = { gList };
				//MethodInfo^ toArray = (System::Linq::Enumerable::typeid)->GetMethod("ToArray")->MakeGenericMethod(aryElemType);
    //            return toArray->Invoke(nullptr, values);
				////return ndatas;
				#pragma endregion
			}

			/// <summary>
			/// ִ��java��������ȡ���ؽ��.
			/// <para>���ؽ��Ϊֵ����,string</para>
			/// </summary>
			/// <param name="objectPtr">java ʵ��ָ�������ָ�� (java.lang.Object/java.lang.Class)</param>
			/// <param name="methodName">java ��������</param>
			/// <param name="valueType">java�˷��ؽ�������ͱ�ʶ</param>
			/// <param name="isStatics">true:��̬��������֮Ϊʵ��������<para>ȱʡΪʵ��������</para></param>
			Object^ GetValueInvokeMethod(IntPtr objectPtr, System::String^ methodName, ValueTypeFlag valueType, ... array<bool,1>^ isStatics)
			{
				if(objectPtr == IntPtr::Zero)
					throw gcnew NullReferenceException("��ָ�����ã��޷���ȡ��Ӧ��java���ͻ����");

				bool isStatic = false;
				int iCount = isStatics->Length;
				if(iCount == 0)
					isStatic = false;
				else
					isStatic = (bool)isStatics->GetValue(0);

				//ֵ���ͷ���ֵ���ٷ�����ú󣬶������ö��󷵻ص� JNI �㡣 ���磺int �� java.lang.Integer
				//ִ��java�˴��� invoke/Bool/Byte/Char/.../Method(Object jobj, String method);

				jmethodID jInvokeMethod = getFromInvokeMap(valueType);
				if(jInvokeMethod == NULL)
				{
					//System::Console::WriteLine("��ȡ invoke/Bool/Byte/Char/.../Method(Object jobj, String method)");
					const char* jLoaderInvokeMethodName = "";
					const char* jLoaderInvokeMethodSign = "";

					#pragma region �������ͱ�ʶ,���� invoke[...]Method �����������ǩ��
					if(valueType == ValueTypeFlag::VF_bool){
						//boolean
						jLoaderInvokeMethodName = "invokeBoolMethod";
						jLoaderInvokeMethodSign = "(Ljava/lang/Object;Ljava/lang/String;)Z";
					}
					else if(valueType == ValueTypeFlag::VF_byte){
						//byte
						jLoaderInvokeMethodName = "invokeByteMethod";
						jLoaderInvokeMethodSign = "(Ljava/lang/Object;Ljava/lang/String;)B";
					}
					else if(valueType == ValueTypeFlag::VF_char){
						//char
						jLoaderInvokeMethodName = "invokeCharMethod";
						jLoaderInvokeMethodSign = "(Ljava/lang/Object;Ljava/lang/String;)C";
					}
					else if(valueType == ValueTypeFlag::VF_short){
						//short
						jLoaderInvokeMethodName = "invokeShortMethod";
						jLoaderInvokeMethodSign = "(Ljava/lang/Object;Ljava/lang/String;)S";
					}
					else if(valueType == ValueTypeFlag::VF_int){
						//int
						jLoaderInvokeMethodName = "invokeIntMethod";
						jLoaderInvokeMethodSign = "(Ljava/lang/Object;Ljava/lang/String;)I";
					}
					else if(valueType == ValueTypeFlag::VF_long){
						//long
						jLoaderInvokeMethodName = "invokeLongMethod";
						jLoaderInvokeMethodSign = "(Ljava/lang/Object;Ljava/lang/String;)J";
					}
					else if(valueType == ValueTypeFlag::VF_float){
						//float
						jLoaderInvokeMethodName = "invokeFloatMethod";
						jLoaderInvokeMethodSign = "(Ljava/lang/Object;Ljava/lang/String;)F";
					}
					else if(valueType == ValueTypeFlag::VF_double){
						//double
						jLoaderInvokeMethodName = "invokeDoubleMethod";
						jLoaderInvokeMethodSign = "(Ljava/lang/Object;Ljava/lang/String;)D";
					}
					else if(valueType == ValueTypeFlag::VF_string){
						//string
						jLoaderInvokeMethodName = "invokeStringMethod";
						jLoaderInvokeMethodSign = "(Ljava/lang/Object;Ljava/lang/String;)Ljava/lang/String;";
					}
					#pragma endregion
				
					jInvokeMethod = jvmrun::getEnv()->GetMethodID(JarLoaderClass, jLoaderInvokeMethodName, jLoaderInvokeMethodSign);
					addToInvokeMap(valueType, jInvokeMethod);
				}

				jstring jMethodName = getJString(methodName);
				#pragma region �������ͱ�ʶ,���ò�ͬ��jni���� Call[����]Method
				if(valueType == ValueTypeFlag::VF_bool){
					jboolean jr = jvmrun::getEnv()->CallBooleanMethod(
							JarLoaderObject,
							jInvokeMethod,
							!isStatic ? (jobject)objectPtr.ToPointer() : (jclass)objectPtr.ToPointer(), 
							jMethodName);
					this->HasJavaExceptionByThrow();
					return jr == 0 ? false : true;
				}
				else if(valueType == ValueTypeFlag::VF_byte){
					jbyte jr = jvmrun::getEnv()->CallByteMethod(
							JarLoaderObject,
							jInvokeMethod,
							!isStatic ? (jobject)objectPtr.ToPointer() : (jclass)objectPtr.ToPointer(), 
							jMethodName);
					this->HasJavaExceptionByThrow();
					return gcnew Byte(jr);
				}
				else if(valueType == ValueTypeFlag::VF_char){
					jchar jr = jvmrun::getEnv()->CallCharMethod(
							JarLoaderObject,
							jInvokeMethod,
							!isStatic ? (jobject)objectPtr.ToPointer() : (jclass)objectPtr.ToPointer(), 
							jMethodName);
					this->HasJavaExceptionByThrow();
					return gcnew Char(jr);
				}
				else if(valueType == ValueTypeFlag::VF_short){
					jshort jr = jvmrun::getEnv()->CallShortMethod(
							JarLoaderObject,
							jInvokeMethod,
							!isStatic ? (jobject)objectPtr.ToPointer() : (jclass)objectPtr.ToPointer(), 
							jMethodName);
					this->HasJavaExceptionByThrow();
					return gcnew System::Int16(jr);
				}
				else if(valueType == ValueTypeFlag::VF_int){
					jint jr = jvmrun::getEnv()->CallIntMethod(
							JarLoaderObject,
							jInvokeMethod,
							!isStatic ? (jobject)objectPtr.ToPointer() : (jclass)objectPtr.ToPointer(), 
							jMethodName);
					this->HasJavaExceptionByThrow();
					return gcnew System::Int32(jr);
				}
				else if(valueType == ValueTypeFlag::VF_long){
					jlong jr = jvmrun::getEnv()->CallLongMethod(
							JarLoaderObject,
							jInvokeMethod,
							!isStatic ? (jobject)objectPtr.ToPointer() : (jclass)objectPtr.ToPointer(), 
							jMethodName);
					this->HasJavaExceptionByThrow();
					return gcnew System::Int64(jr);
				}
				else if(valueType == ValueTypeFlag::VF_float){
					jfloat jr = jvmrun::getEnv()->CallFloatMethod(
							JarLoaderObject,
							jInvokeMethod,
							!isStatic ? (jobject)objectPtr.ToPointer() : (jclass)objectPtr.ToPointer(), 
							jMethodName);
					this->HasJavaExceptionByThrow();
					return gcnew System::Single(jr);
				}
				else if(valueType == ValueTypeFlag::VF_double){
					jdouble jr = jvmrun::getEnv()->CallDoubleMethod(
							JarLoaderObject,
							jInvokeMethod,
							!isStatic ? (jobject)objectPtr.ToPointer() : (jclass)objectPtr.ToPointer(), 
							jMethodName);
					this->HasJavaExceptionByThrow();
					return gcnew System::Double(jr);
				}
				else if(valueType == ValueTypeFlag::VF_string){
					jstring jstr = (jstring)jvmrun::getEnv()->CallObjectMethod(
							JarLoaderObject,
							jInvokeMethod,
							!isStatic ? (jobject)objectPtr.ToPointer() : (jclass)objectPtr.ToPointer(), 
							jMethodName);
					this->HasJavaExceptionByThrow();
					if(jstr != NULL)
						return gcnew String(lconvert::j2string(jstr));
				}
				#pragma endregion

				return nullptr;
			}

			/// <summary>
			/// ִ��java��������ȡ������.
			/// <para>int��ֵ��������,��String����</para>
			/// </summary>
			Object^ GetValueArrayInvokeMethod(IntPtr objectPtr, System::String^ methodName, ValueTypeFlag valueType, ... array<bool,1>^ isStatics)
			{
				if(objectPtr == IntPtr::Zero)
					throw gcnew NullReferenceException("��ָ�����ã��޷���ȡ��Ӧ��java���ͻ����");

				IntPtr resultPtr = GetObjectInvokeMethod(objectPtr, methodName, isStatics);
				jobject resultObj = (jobject)resultPtr.ToPointer();

				return this->ConvertJResultToArray(resultObj, false, nullptr, valueType);
			}

			void SetFieldInvokeMethod(IntPtr objectPtr, System::String^ fieldName,... array<bool,1>^ isStatics)
			{
				bool isStatic = false;
				int iCount = isStatics->Length;
				if(iCount == 0)
					isStatic = false;
				else
					isStatic = (bool)isStatics->GetValue(0);

				jstring jfldName = getJString(fieldName);
				jmethodID jInvokeSetFldMethod = jvmrun::getEnv()->GetMethodID(JarLoaderClass, "invokeObjectField", "(Ljava/lang/Object;Ljava/lang/String;Z)Ljava/lang/Object;");
				jvmrun::getEnv()->CallObjectMethod(JarLoaderObject,jInvokeSetFldMethod,(jobject)objectPtr.ToPointer(),jfldName, true);
			}

			Object^ GetFieldInvokeMethod(IntPtr objectPtr, System::String^ fieldName, ValueTypeFlag valueType, ... array<bool,1>^ isStatics)
			{
				if(objectPtr == IntPtr::Zero)
					throw gcnew NullReferenceException("��ָ�����ã��޷���ȡ��Ӧ��java���ͻ����");

				bool isStatic = false;
				int iCount = isStatics->Length;
				if(iCount == 0)
					isStatic = false;
				else
					isStatic = (bool)isStatics->GetValue(0);

				jmethodID jInvokeMethod = getFromFieldMap(valueType);
				if(jInvokeMethod == NULL)
				{
					//System::Console::WriteLine("��ȡ invoke/Bool/Byte/Char/.../Field(Object jobj, String fieldName)");
					const char* jLoaderInvokeMethodName = "";
					const char* jLoaderInvokeMethodSign = "";

					#pragma region �������ͱ�ʶ,���� invoke[...]Field �����������ǩ��
					if(valueType == ValueTypeFlag::VF_bool){
						//boolean
						jLoaderInvokeMethodName = "invokeBoolField";
						jLoaderInvokeMethodSign = "(Ljava/lang/Object;Ljava/lang/String;)Z";
					}
					else if(valueType == ValueTypeFlag::VF_byte){
						//byte
						jLoaderInvokeMethodName = "invokeByteField";
						jLoaderInvokeMethodSign = "(Ljava/lang/Object;Ljava/lang/String;)B";
					}
					else if(valueType == ValueTypeFlag::VF_char){
						//char
						jLoaderInvokeMethodName = "invokeCharField";
						jLoaderInvokeMethodSign = "(Ljava/lang/Object;Ljava/lang/String;)C";
					}
					else if(valueType == ValueTypeFlag::VF_short){
						//short
						jLoaderInvokeMethodName = "invokeShortField";
						jLoaderInvokeMethodSign = "(Ljava/lang/Object;Ljava/lang/String;)S";
					}
					else if(valueType == ValueTypeFlag::VF_int){
						//int
						jLoaderInvokeMethodName = "invokeIntField";
						jLoaderInvokeMethodSign = "(Ljava/lang/Object;Ljava/lang/String;)I";
					}
					else if(valueType == ValueTypeFlag::VF_long){
						//long
						jLoaderInvokeMethodName = "invokeLongField";
						jLoaderInvokeMethodSign = "(Ljava/lang/Object;Ljava/lang/String;)J";
					}
					else if(valueType == ValueTypeFlag::VF_float){
						//float
						jLoaderInvokeMethodName = "invokeFloatField";
						jLoaderInvokeMethodSign = "(Ljava/lang/Object;Ljava/lang/String;)F";
					}
					else if(valueType == ValueTypeFlag::VF_double){
						//double
						jLoaderInvokeMethodName = "invokeDoubleField";
						jLoaderInvokeMethodSign = "(Ljava/lang/Object;Ljava/lang/String;)D";
					}
					else if(valueType == ValueTypeFlag::VF_string){
						//string
						jLoaderInvokeMethodName = "invokeStringField";
						jLoaderInvokeMethodSign = "(Ljava/lang/Object;Ljava/lang/String;)Ljava/lang/String;";
					}
					else if(valueType == ValueTypeFlag::VF_object){
						//string
						jLoaderInvokeMethodName = "invokeObjectField";
						jLoaderInvokeMethodSign = "(Ljava/lang/Object;Ljava/lang/String;Z)Ljava/lang/Object;";
					}
					#pragma endregion
				
					jInvokeMethod = jvmrun::getEnv()->GetMethodID(JarLoaderClass, jLoaderInvokeMethodName, jLoaderInvokeMethodSign);
					addToFieldMap(valueType, jInvokeMethod);
				}

				jstring jFieldName = getJString(fieldName);
				#pragma region �������ͱ�ʶ,���ò�ͬ��jni���� Call[����]Method
				if(valueType == ValueTypeFlag::VF_bool){
					jboolean jr = jvmrun::getEnv()->CallBooleanMethod(
							JarLoaderObject,
							jInvokeMethod,
							!isStatic ? (jobject)objectPtr.ToPointer() : (jclass)objectPtr.ToPointer(), 
							jFieldName);
					this->HasJavaExceptionByThrow();
					return jr == 0 ? false : true;
				}
				else if(valueType == ValueTypeFlag::VF_byte){
					jbyte jr = jvmrun::getEnv()->CallByteMethod(
							JarLoaderObject,
							jInvokeMethod,
							!isStatic ? (jobject)objectPtr.ToPointer() : (jclass)objectPtr.ToPointer(), 
							jFieldName);
					this->HasJavaExceptionByThrow();
					return gcnew Byte(jr);
				}
				else if(valueType == ValueTypeFlag::VF_char){
					jchar jr = jvmrun::getEnv()->CallCharMethod(
							JarLoaderObject,
							jInvokeMethod,
							!isStatic ? (jobject)objectPtr.ToPointer() : (jclass)objectPtr.ToPointer(), 
							jFieldName);
					this->HasJavaExceptionByThrow();
					return gcnew Char(jr);
				}
				else if(valueType == ValueTypeFlag::VF_short){
					jshort jr = jvmrun::getEnv()->CallShortMethod(
							JarLoaderObject,
							jInvokeMethod,
							!isStatic ? (jobject)objectPtr.ToPointer() : (jclass)objectPtr.ToPointer(), 
							jFieldName);
					this->HasJavaExceptionByThrow();
					return gcnew System::Int16(jr);
				}
				else if(valueType == ValueTypeFlag::VF_int){
					jint jr = jvmrun::getEnv()->CallIntMethod(
							JarLoaderObject,
							jInvokeMethod,
							!isStatic ? (jobject)objectPtr.ToPointer() : (jclass)objectPtr.ToPointer(), 
							jFieldName);
					this->HasJavaExceptionByThrow();
					return gcnew System::Int32(jr);
				}
				else if(valueType == ValueTypeFlag::VF_long){
					jlong jr = jvmrun::getEnv()->CallLongMethod(
							JarLoaderObject,
							jInvokeMethod,
							!isStatic ? (jobject)objectPtr.ToPointer() : (jclass)objectPtr.ToPointer(), 
							jFieldName);
					this->HasJavaExceptionByThrow();
					return gcnew System::Int64(jr);
				}
				else if(valueType == ValueTypeFlag::VF_float){
					jfloat jr = jvmrun::getEnv()->CallFloatMethod(
							JarLoaderObject,
							jInvokeMethod,
							!isStatic ? (jobject)objectPtr.ToPointer() : (jclass)objectPtr.ToPointer(), 
							jFieldName);
					this->HasJavaExceptionByThrow();
					return gcnew System::Single(jr);
				}
				else if(valueType == ValueTypeFlag::VF_double){
					jdouble jr = jvmrun::getEnv()->CallDoubleMethod(
							JarLoaderObject,
							jInvokeMethod,
							!isStatic ? (jobject)objectPtr.ToPointer() : (jclass)objectPtr.ToPointer(), 
							jFieldName);
					this->HasJavaExceptionByThrow();
					return gcnew System::Double(jr);
				}
				else if(valueType == ValueTypeFlag::VF_string){
					jstring jstr = (jstring)jvmrun::getEnv()->CallObjectMethod(
							JarLoaderObject,
							jInvokeMethod,
							!isStatic ? (jobject)objectPtr.ToPointer() : (jclass)objectPtr.ToPointer(), 
							jFieldName);
					this->HasJavaExceptionByThrow();
					if(jstr != NULL)
						return gcnew String(lconvert::j2string(jstr));
				}
				else if(valueType == ValueTypeFlag::VF_object){
					jobject jobjR = jvmrun::getEnv()->CallObjectMethod(
							JarLoaderObject,
							jInvokeMethod,
							!isStatic ? (jobject)objectPtr.ToPointer() : (jclass)objectPtr.ToPointer(), 
							jFieldName,
							false); //false:��ʾȡֵ
					this->HasJavaExceptionByThrow();
					if(jobjR != NULL)
						return IntPtr(jobjR);
				}
				#pragma endregion

				return nullptr;
			}

			Object^ GetFieldArrayInvokeMethod(IntPtr objectPtr,bool isLangObject, Type^ NObjType, System::String^ fieldName, ValueTypeFlag valueType, ... array<bool,1>^ isStatics)
			{
				if(objectPtr == IntPtr::Zero)
					throw gcnew NullReferenceException("��ָ�����ã��޷���ȡ��Ӧ��java���ͻ����");

				bool isStatic = false;
				int iCount = isStatics->Length;
				if(iCount == 0)
					isStatic = false;
				else
					isStatic = (bool)isStatics->GetValue(0);

				jmethodID jInvokeMethod = getFromFieldMap(ValueTypeFlag::VF_object);
				if(jInvokeMethod == NULL)
				{
					//System::Console::WriteLine("��ȡ invokeObjectField(Object jobj, String fieldName)");
					const char* jLoaderInvokeMethodName = "invokeObjectField";
					const char* jLoaderInvokeMethodSign = "(Ljava/lang/Object;Ljava/lang/String;Z)Ljava/lang/Object;";

					jInvokeMethod = jvmrun::getEnv()->GetMethodID(JarLoaderClass, jLoaderInvokeMethodName, jLoaderInvokeMethodSign);
					addToFieldMap(ValueTypeFlag::VF_object, jInvokeMethod);
				}

				jstring jFieldName = getJString(fieldName);
				jobject resultObj = (jstring)jvmrun::getEnv()->CallObjectMethod(
							JarLoaderObject,
							jInvokeMethod,
							!isStatic ? (jobject)objectPtr.ToPointer() : (jclass)objectPtr.ToPointer(), 
							jFieldName,
							false);
				this->HasJavaExceptionByThrow();

				return this->ConvertJResultToArray(resultObj,isLangObject, NObjType, valueType);
			}

			/// <summary>
			/// JVM �ͷ� java ʵ��
			/// </summary>
			/// <param name="objectPtr">��Ҫ�ͷŵ� java ʵ��ָ�� (java.lang.Object)</param>
			void FreeJObject(Object^ IJObjectface)
			{
				if(IJObjectface == nullptr)return;
				try
				{
					IObject^ ijobj = (IObject^)IJObjectface;
					if(ijobj->JObjectPtr != IntPtr::Zero){
						jobject jo = (jobject)ijobj->JObjectPtr.ToPointer();
						jvmrun::getEnv()->DeleteLocalRef(jo);
						jo = NULL;
						ijobj->setJObjectPtr(IntPtr::Zero);
					}

					if(ijobj->JClassPtr != IntPtr::Zero){
						jclass jc = (jclass)ijobj->JClassPtr.ToPointer();
						jvmrun::getEnv()->DeleteLocalRef(jc);
						jc = NULL;
						ijobj->setJClassPtr(IntPtr::Zero);
					}
				}
				catch(...){
				}
			}

			/// <summary>
			/// ת�����йܶ���
			/// </summary>
			/// <param name="objectPtr">java ʵ��ָ�� (java.lang.Object)</param>
			/// <param name="NObjType">java ʵ����Ӧ���й����͡�</param>
			Object^ CastToNObject(IntPtr objectPtr, Type^ NObjType)
			{
				jobject jo = (jobject)objectPtr.ToPointer();
				IntPtr classPtr = IntPtr(jvmrun::getEnv()->GetObjectClass(jo));
				//array<Object^>^ values = gcnew array<Object^>(2);
				//values[0] = objectPtr;
				//values[1] = classPtr;
				////values[2] = this;

				//array<Type^,1>^ types = gcnew array<Type^>(2);
				//types[0] = classPtr.GetType();
				//types[1] = classPtr.GetType();
				////types[2] = this->GetType();

				array<Object^>^ values = { objectPtr, classPtr };
				array<Type^,1>^ types = { IntPtr::typeid, IntPtr::typeid };
				ConstructorInfo^ ctor = NObjType->GetConstructor(
                    BindingFlags::CreateInstance | BindingFlags::NonPublic | BindingFlags::Default | BindingFlags::Instance ,
                    nullptr ,types, nullptr);

				return ctor->Invoke(values);
			}

			/// <summary>
			/// �������ֵ,ת�����йܶ���ɿ����͵�ֵ
			/// </summary>
			/// <param name="valueTypePtr">java ֵ���Ͷ�Ӧʵ��ָ�� (java.lang.Integer)</param>
			/// <param name="ValueTypeFlag">���ͱ�ʶö��ֵ��</param>
			Object^ CastToNValueNullable(IntPtr valueTypePtr, ValueTypeFlag vtFlag)
			{
				jobject jo = (jobject)valueTypePtr.ToPointer();
				if(vtFlag == ValueTypeFlag::VF_string){
					jstring jr =  (jstring)jo;
					char* str = lconvert::j2string(jr);
					return gcnew String(str);
				}

				jclass jvalueclass = jvmrun::getEnv()->GetObjectClass(jo);
				if(vtFlag == ValueTypeFlag::VF_bool)
				{
					jmethodID jmValueOf = jvmrun::getEnv()->GetMethodID(jvalueclass, "booleanValue", "()Z");
					jboolean jr = jvmrun::getEnv()->CallBooleanMethod(jo, jmValueOf);
					return jr == 0 ? false : true;
				}
				else if(vtFlag == ValueTypeFlag::VF_byte)
				{
					jmethodID jmValueOf = jvmrun::getEnv()->GetMethodID(jvalueclass, "byteValue", "()B");
					jbyte jr = jvmrun::getEnv()->CallByteMethod(jo, jmValueOf);
					return gcnew Byte(jr);
				}
				else if(vtFlag == ValueTypeFlag::VF_char)
				{
					jmethodID jmValueOf = jvmrun::getEnv()->GetMethodID(jvalueclass, "charValue", "()C");
					jchar jr = jvmrun::getEnv()->CallCharMethod(jo, jmValueOf);
					return gcnew Char(jr);
				}		
				else if(vtFlag == ValueTypeFlag::VF_short)
				{
					jmethodID jmValueOf = jvmrun::getEnv()->GetMethodID(jvalueclass, "shortValue", "()S");
					jshort jr = jvmrun::getEnv()->CallShortMethod(jo, jmValueOf);
					return gcnew System::Int16(jr);
				}
				else if(vtFlag == ValueTypeFlag::VF_int)
				{
					jmethodID jmValueOf = jvmrun::getEnv()->GetMethodID(jvalueclass, "intValue", "()I");
					jint jr = jvmrun::getEnv()->CallIntMethod(jo, jmValueOf);
					return gcnew System::Int32(jr);
				}
				else if(vtFlag == ValueTypeFlag::VF_long)
				{
					jmethodID jmValueOf = jvmrun::getEnv()->GetMethodID(jvalueclass, "longValue", "()J");
					jlong jr = jvmrun::getEnv()->CallLongMethod(jo, jmValueOf);
					return gcnew System::Int64(jr);
				}
				else if(vtFlag == ValueTypeFlag::VF_float)
				{
					jmethodID jmValueOf = jvmrun::getEnv()->GetMethodID(jvalueclass, "floatValue", "()F");
					jfloat jr = jvmrun::getEnv()->CallFloatMethod(jo, jmValueOf);
					return gcnew System::Single(jr);
				}
				else if(vtFlag == ValueTypeFlag::VF_double)
				{
					jmethodID jmValueOf = jvmrun::getEnv()->GetMethodID(jvalueclass, "doubleValue", "()D");
					jdouble jr = jvmrun::getEnv()->CallDoubleMethod(jo, jmValueOf);
					gcnew System::Double(jr);
				}

				return nullptr;
			}

			/// <summary>
			/// java.object.toString()
			/// </summary>
			String^ GetObjectToString(IntPtr objectPtr)
			{
				jobject jobj = (jobject)objectPtr.ToPointer();
				jmethodID jToStringMethod = jvmrun::getEnv()->GetMethodID(JarLoaderClass, "getInstanceToString","(Ljava/lang/Object;)Ljava/lang/String;");
				jstring jstr = (jstring)jvmrun::getEnv()->CallObjectMethod(JarLoaderObject,jToStringMethod, jobj);
				this->HasJavaExceptionByThrow();
				char* str = lconvert::j2string(jstr);
				return gcnew String(str);
			}

			/// <summary>
			/// java.object.hashCode()
			/// </summary>
			int GetObjectHashcode(IntPtr objectPtr)
			{
				jobject jobj = (jobject)objectPtr.ToPointer();
				jmethodID jToHashMethod = jvmrun::getEnv()->GetMethodID(JarLoaderClass, "getHashCode","(Ljava/lang/Object;)I");
				jint iHashcode = jvmrun::getEnv()->CallIntMethod(JarLoaderObject,jToHashMethod, jobj);
				this->HasJavaExceptionByThrow();
				return iHashcode;
			}

		private:
			Object^ ConvertJResultToArray(jobject resultObj, bool isLangObject, Type^ NObjType, ValueTypeFlag valueType)
			{
				System::Collections::IList^ gList = nullptr;
				Type^ aryElemType = nullptr;

				if(valueType == ValueTypeFlag::VF_bool){
					#pragma region Boolean
					if(isLangObject){
						gList = gcnew System::Collections::Generic::List<Nullable<Boolean>>();
						aryElemType = Nullable<Boolean>::typeid;
					}
					else{
						if(resultObj == NULL)
							return gcnew array<System::Boolean,1>(0);

						jbooleanArray* arr = reinterpret_cast<jbooleanArray*>(&resultObj);
						int size = jvmrun::getEnv()->GetArrayLength(*arr);
						array<System::Boolean,1>^ ndatas = gcnew array<System::Boolean,1>(size);

						jboolean * data = jvmrun::getEnv()->GetBooleanArrayElements(*arr, NULL);
						for (int i = 0; i < size; i++) {
							ndatas[i] = data[i] == 0 ? false : true;
						}
						jvmrun::getEnv()->ReleaseBooleanArrayElements(*arr, data, 0); 
						return ndatas;
					}
					#pragma endregion
				}
				else if(valueType == ValueTypeFlag::VF_byte){
					#pragma region Char
					if(isLangObject){
						gList = gcnew System::Collections::Generic::List<Nullable<Byte>>();
						aryElemType = Nullable<Byte>::typeid;
					}
					else{
						if(resultObj == NULL)
							return gcnew array<System::Byte,1>(0);

						jbyteArray* arr = reinterpret_cast<jbyteArray*>(&resultObj);
						int size = jvmrun::getEnv()->GetArrayLength(*arr);
						array<System::Byte,1>^ ndatas = gcnew array<System::Byte,1>(size);

						jbyte * data = jvmrun::getEnv()->GetByteArrayElements(*arr, NULL);
						for (int i = 0; i < size; i++) {
							ndatas[i] = data[i];
						}
						jvmrun::getEnv()->ReleaseByteArrayElements(*arr, data, 0); 
						return ndatas;
					}
					#pragma endregion
				}
				else if(valueType == ValueTypeFlag::VF_char){
					#pragma region Char
					if(isLangObject){
						gList = gcnew System::Collections::Generic::List<Nullable<Char>>();
						aryElemType = Nullable<Char>::typeid;
					}
					else{
						if(resultObj == NULL)
							return gcnew array<System::Char,1>(0);

						jcharArray* arr = reinterpret_cast<jcharArray*>(&resultObj);
						int size = jvmrun::getEnv()->GetArrayLength(*arr);
						array<System::Char,1>^ ndatas = gcnew array<System::Char,1>(size);

						jchar * data = jvmrun::getEnv()->GetCharArrayElements(*arr, NULL);
						for (int i = 0; i < size; i++) {
							ndatas[i] = data[i];
						}
						jvmrun::getEnv()->ReleaseCharArrayElements(*arr, data, 0); 
						return ndatas;
					}
					#pragma endregion
				}
				else if(valueType == ValueTypeFlag::VF_short){
					#pragma region int16
					if(isLangObject){
						gList = gcnew System::Collections::Generic::List<Nullable<Int16>>();
						aryElemType = Nullable<Int16>::typeid;
					}
					else{
						if(resultObj == NULL)
							return gcnew array<System::Int16,1>(0);

						jshortArray* arr = reinterpret_cast<jshortArray*>(&resultObj);
						int size = jvmrun::getEnv()->GetArrayLength(*arr);
						array<System::Int16,1>^ ndatas = gcnew array<System::Int16,1>(size);

						short * data = jvmrun::getEnv()->GetShortArrayElements(*arr, NULL);
						for (int i = 0; i < size; i++) {
							ndatas[i] = data[i];
						}
						jvmrun::getEnv()->ReleaseShortArrayElements(*arr, data, 0); 
						return ndatas;
					}
					#pragma endregion
				}
				else if(valueType == ValueTypeFlag::VF_int){
					#pragma region int32
					if(isLangObject){
						gList = gcnew System::Collections::Generic::List<Nullable<Int32>>();
						aryElemType = Nullable<Int32>::typeid;
					}
					else{
						if(resultObj == NULL)
							return gcnew array<System::Int32,1>(0);
					
						jintArray* arr = reinterpret_cast<jintArray*>(&resultObj);
						int size = jvmrun::getEnv()->GetArrayLength(*arr);
						array<System::Int32,1>^ ndatas = gcnew array<System::Int32,1>(size);

						jint * data = jvmrun::getEnv()->GetIntArrayElements(*arr, NULL);
						for (int i = 0; i < size; i++) {
							ndatas[i] = data[i];
						}
						jvmrun::getEnv()->ReleaseIntArrayElements(*arr, data, 0); 
						return ndatas;
					}
					#pragma endregion
				}
				else if(valueType == ValueTypeFlag::VF_long){
					#pragma region int64
					if(isLangObject){
						gList = gcnew System::Collections::Generic::List<Nullable<Int64>>();
						aryElemType = Nullable<Int64>::typeid;
					}
					else{
						if(resultObj == NULL)
							return gcnew array<System::Int64,1>(0);

						jlongArray* arr = reinterpret_cast<jlongArray*>(&resultObj);
						int size = jvmrun::getEnv()->GetArrayLength(*arr);
						array<System::Int64,1>^ ndatas = gcnew array<System::Int64,1>(size);

						jlong * data = jvmrun::getEnv()->GetLongArrayElements(*arr, NULL);
						for (int i = 0; i < size; i++) {
							ndatas[i] = data[i];
						}
						jvmrun::getEnv()->ReleaseLongArrayElements(*arr, data, 0); 
						return ndatas;
					}
					#pragma endregion
				}
				else if(valueType == ValueTypeFlag::VF_float)
				{
					#pragma region single
					if(isLangObject){
						gList = gcnew System::Collections::Generic::List<Nullable<Single>>();
						aryElemType = Nullable<Single>::typeid;
					}
					else{
						if(resultObj == NULL)
							return gcnew array<System::Single,1>(0);

						jfloatArray* arr = reinterpret_cast<jfloatArray*>(&resultObj);
						int size = jvmrun::getEnv()->GetArrayLength(*arr);
						array<System::Single,1>^ ndatas = gcnew array<System::Single,1>(size);

						float * data = jvmrun::getEnv()->GetFloatArrayElements(*arr, NULL);
						for (int i = 0; i < size; i++) {
							ndatas[i] = data[i];
						}
						jvmrun::getEnv()->ReleaseFloatArrayElements(*arr, data, 0); 
						return ndatas;
					}
					#pragma endregion
				}
				else if(valueType == ValueTypeFlag::VF_double){
					#pragma region double
					if(isLangObject){
						gList = gcnew System::Collections::Generic::List<Nullable<Double>>();
						aryElemType = Nullable<Double>::typeid;
					}
					else{
						if(resultObj == NULL)
							return gcnew array<System::Double,1>(0);

						jdoubleArray* arr = reinterpret_cast<jdoubleArray*>(&resultObj);
						int size = jvmrun::getEnv()->GetArrayLength(*arr);
						array<System::Double,1>^ ndatas = gcnew array<System::Double,1>(size);

						double * data = jvmrun::getEnv()->GetDoubleArrayElements(*arr, NULL);
						for (int i = 0; i < size; i++) {
							ndatas[i] = data[i];
						}
						jvmrun::getEnv()->ReleaseDoubleArrayElements(*arr, data, 0); 
						return ndatas;
					}
					#pragma endregion
				}
				else if(valueType == ValueTypeFlag::VF_string){
					#pragma region string
					if(resultObj == NULL)
						return gcnew array<System::String^,1>(0);

					jobjectArray* arr = reinterpret_cast<jobjectArray*>(&resultObj);
					int size = jvmrun::getEnv()->GetArrayLength(*arr);
					array<System::String^,1>^ ndatas = gcnew array<System::String^,1>(size);

					for (int i = 0; i < size; i++) {
						jstring data = (jstring)jvmrun::getEnv()->GetObjectArrayElement(*arr, i);
						if(data != NULL)
							ndatas[i] = gcnew String(lconvert::j2string(data));
					}
					return ndatas;
					#pragma endregion
				}
				else if(valueType == ValueTypeFlag::VF_object){

					//���ͷ���ֵʱ��valueType��ΪVF_object
					if(NObjType == System::Boolean::typeid || NObjType == System::Nullable<System::Boolean>::typeid) 
						return ConvertJResultToArray(resultObj,true, nullptr, ValueTypeFlag::VF_bool);
					else if(NObjType == System::Byte::typeid || NObjType == System::Nullable<System::Byte>::typeid) 
						return ConvertJResultToArray(resultObj,true, nullptr, ValueTypeFlag::VF_byte);
					else if(NObjType == System::Char::typeid || NObjType == System::Nullable<System::Char>::typeid) 
						return ConvertJResultToArray(resultObj,true, nullptr, ValueTypeFlag::VF_char);
					else if(NObjType == Int16::typeid || NObjType == System::Nullable<System::Int16>::typeid) 
						return ConvertJResultToArray(resultObj,true, nullptr, ValueTypeFlag::VF_short);
					else if(NObjType == Int32::typeid || NObjType == System::Nullable<System::Int32>::typeid) 
						return ConvertJResultToArray(resultObj,true, nullptr, ValueTypeFlag::VF_int);
					else if(NObjType == Int64::typeid || NObjType == System::Nullable<System::Int64>::typeid) 
						return ConvertJResultToArray(resultObj,true, nullptr, ValueTypeFlag::VF_long);
					else if(NObjType == System::Single::typeid || NObjType == System::Nullable<System::Single>::typeid) 
						return ConvertJResultToArray(resultObj,true, nullptr, ValueTypeFlag::VF_float);
					else if(NObjType == System::Double::typeid || NObjType == System::Nullable<System::Double>::typeid) 
						return ConvertJResultToArray(resultObj,true, nullptr, ValueTypeFlag::VF_double);
					else if(NObjType == String::typeid) 
						return ConvertJResultToArray(resultObj,isLangObject, NObjType, ValueTypeFlag::VF_string);

					if(resultObj == NULL)
						return gcnew array<System::Object^,1>(0);

					Type^ lstGType = System::Collections::Generic::List::typeid;
					Object^ objList = Activator::CreateInstance(lstGType->MakeGenericType(NObjType));
					gList = (System::Collections::IList^)objList;
					aryElemType = NObjType;
				}

				
				if(isLangObject){
					//string����Ϊ��������ת��
					array<Object^>^ values = { gList };
					MethodInfo^ toArray = (System::Linq::Enumerable::typeid)->GetMethod("ToArray")->MakeGenericMethod(aryElemType);
					if(resultObj == NULL)
						return toArray->Invoke(nullptr, values);

					jobjectArray* arr = reinterpret_cast<jobjectArray*>(&resultObj);
					int size = jvmrun::getEnv()->GetArrayLength(*arr);
					for (int i = 0; i < size; i++) {
						jobject data = jvmrun::getEnv()->GetObjectArrayElement(*arr, i);
						if(data == NULL){
							gList->Add(nullptr);
							continue;
						}

						if(NObjType == nullptr)
							gList->Add(CastToNValueNullable(IntPtr(data), valueType));
						else
							gList->Add(CastToNObject(IntPtr(data), NObjType));
					}
					return toArray->Invoke(nullptr, values);
				}

				return nullptr;
			}
		};
	}
}

