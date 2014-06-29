//#pragma once
//
//using namespace System;
//
//namespace NXDO {
//	namespace JBridge{
//
//ref class ExtOper
//{
//		public:
//			template<class T>
//			static Boolean IS(Object^ u);
//
//			template<class T, class U>
//			static T AS(U u);
//		};
//	}
//}

template<class T,class U>
static Boolean IS(U u){
	return dynamic_cast<T^>(u) != nullptr;
}

