package jxdo.rjava;

import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.lang.reflect.Modifier;
import java.net.URL;
import java.net.URLClassLoader;
import java.util.ArrayList;
import java.util.Enumeration;
import java.util.List;
import java.util.jar.JarEntry;
import java.util.jar.JarFile;

class JCSharp {
	static String crlf;
	static ClassLoader clazzLoader;
	List<String> lstClassNames;
	
	public JCSharp(String jarNames) throws Throwable{
		if(crlf == null)crlf= System.getProperty("line.separator");		
		lstClassNames = new ArrayList<String>();
		if(clazzLoader == null)		
			clazzLoader = this.getJarClassLoader(jarNames, this.getClass().getClassLoader());
	}
	
	@SuppressWarnings("unused")
	private void addClassLaoder(String jarNames) throws Throwable {
		lstClassNames.clear();
		ClassLoader newClazzLoader = this.getJarClassLoader(jarNames, clazzLoader);
		if(newClazzLoader != clazzLoader)clazzLoader = newClazzLoader;
	}

	private ClassLoader getJarClassLoader(String jarNames, ClassLoader parentClassLoader) throws Throwable{				
		List<URL> lstUrls = new ArrayList<URL>();
		for(String jarName : jarNames.split(";")){	
			//System.out.println(jarName);
			
			File file  = new File(jarName);
			if(file.getName().compareToIgnoreCase("jxdo.rjava.jar") == 0)continue;
			if(!file.exists())
			{
				java.io.FileNotFoundException exp = new java.io.FileNotFoundException(jarName);
				if(exp.getMessage() == null){
					Throwable ta = exp.getCause();
					if(ta!=null)throw ta;
				}
				throw exp;
			}
			
			this.fillClassNames(jarName);
			
			URL jarUrl = new URL("jar", "","file:" + file.getAbsolutePath()+"!/"); 
			lstUrls.add(jarUrl);
		}
		
		if(lstUrls.size()==0)
			return parentClassLoader;
		
		URL[] urls = lstUrls.toArray(new URL[0]);				
		return URLClassLoader.newInstance(urls, parentClassLoader);
	}

	private void fillClassNames(String jarFileName) throws Throwable{
		JarFile jar = new JarFile(jarFileName);
		Enumeration<JarEntry> entries = jar.entries();
        while (entries.hasMoreElements()) {
            String entry = entries.nextElement().getName();
            if(!entry.endsWith(".class"))continue;
            //System.out.println(entry);
            
            String clsName = entry.replaceAll(".class", "").replaceAll("/", ".");
            lstClassNames.add(clsName);
        }
	}

	List<Class<?>> lstClasses;
	public void fillClasses(String directoryName) throws Throwable{
		if(lstClasses == null)
			lstClasses = new ArrayList<Class<?>>();
		else
			lstClasses.clear();
		
		//System.out.println(directoryName);
		

		List<ClassFile> lst = new ArrayList<ClassFile>();		
		for(int i=0;i<lstClassNames.size();i++){	
			String name = lstClassNames.get(i);
			if(name.indexOf("$") > 0){
				continue;
			}

			ClassFile cd = new ClassFile();	
			cd.className = name;
			lst.add(cd);
			
			lstClassNames.remove(i);
			i-=1;
		}


		java.io.File f = new File(directoryName);
		if(!f.exists())f.mkdirs();
		
		lstFileWriters = new ArrayList<FileWriter>();
		this.ForEachCoder(lst,directoryName);
		for(FileWriter fw : lstFileWriters)
			fw.close();
	}
	
	List<FileWriter> lstFileWriters;
	private void ForEachCoder(List<ClassFile> lst, String directoryName){
		for(ClassFile cf : lst){
			Class<?> clazz = cf.loadClass();
			if(clazz == null)continue;

			//C#端的接口不支持嵌套
			boolean isInterface = clazz.isInterface();
			FileWriter fwCurrent = isInterface ? cf.getFileWriter(directoryName) : cf.getParent(cf).getFileWriter(directoryName);
			String tabs = isInterface ? "\t" : cf.getTabs();
			
			if(!lstFileWriters.contains(fwCurrent))
				lstFileWriters.add(fwCurrent);
			
			JCSharpOuter jcOuter = new JCSharpOuter(clazz, fwCurrent, isInterface ? false : cf.Postion > 0);
			jcOuter.writerNamespaceStart(tabs);			
			jcOuter.writerDefineStart();
			
			System.out.println(tabs + cf.className);
			this.ForEachCoder(cf.getNetseds(), directoryName);
			
			jcOuter.writerDefineEnd();
			jcOuter.writerNamespaceEnd();
			
			
		}
	}
	
	public class ClassFile{
		String className;
		int Postion = 0;
		ClassFile Parent;
				
		public List<ClassFile> getNetseds(){
			List<ClassFile> netseds = new ArrayList<ClassFile>();
			for(int i=0;i<lstClassNames.size();i++){	
				String ns = lstClassNames.get(i);
				if(!ns.startsWith(className))continue;
				
				int level = ns.split("\\$").length - 1;
				if(level == Postion +1)
				{
					ClassFile cd = new ClassFile();
					cd.className = ns;
					cd.Postion = level;
					cd.Parent = this;
					
					netseds.add(cd);
					lstClassNames.remove(i);
					i-=1;
				}
			}
			return netseds;
		}
		
		ClassFile cfSearchParent;
		private ClassFile getParent(ClassFile cf){
			if(cfSearchParent == null)
				searchParent(cf);
			return cfSearchParent;
		}
		
		private void searchParent(ClassFile cf){
			if(cf.Parent == null){
				cfSearchParent = cf;
				return;
			}
			
			searchParent(cf.Parent);
			//cf.Level += 1;
		}
		
		Class<?> cls;
		public Class<?> loadClass(){
			if(cls == null){
				try {
					cls = clazzLoader.loadClass(this.className);
				} catch (ClassNotFoundException e) {
					return null;
				}
	
				int im = cls.getModifiers();
				if(!Modifier.isPublic(im))
					return null;
			}
			return cls;
		}

		String _tabs;
		public String getTabs(){
			if(_tabs == null){
				String s = "\t";
				for(int i=0;i<this.Postion;i++)
					s += "\t";
				_tabs = s;
			}
			return _tabs;
		}
	
		FileWriter fw;
		public FileWriter getFileWriter(String directoryName){
			if(fw == null){
				String csFileName = className.replace('$', '.');
//				if(csFileName.indexOf(".") > -1)
//					csFileName = csFileName.substring(csFileName.indexOf(".") + 1, csFileName.length() + 2  - csFileName.indexOf("."));
//				if(csFileName.indexOf("$") > -1)
//					csFileName = csFileName.substring(csFileName.indexOf("$") + 1, csFileName.length() + 2 - csFileName.indexOf("$"));
				
				File f = new File(directoryName + File.separator + csFileName + ".cs");
				if(f.exists())f.delete();
				boolean isCreated;
				try {
					isCreated = f.createNewFile();
				} catch (IOException e) {
					isCreated = false;
				}
				if(!isCreated)return null;
				
				try {
					fw = new FileWriter(f);
				} catch (IOException e) {
					return null;
				}	
			}
			return fw;
		}
	}
	
	public static void main(String[] args) throws Throwable {
		//home
//		String fname = "E:\\DotNet2010\\NXDO.Mixed\\NXDO.Mixed.V2015\\Tester\\RJavaX64\\bin\\Debug\\jforloaded.jar";
//		JCSharp jf = new JCSharp(fname);
//		jf.fillClasses("E:\\DotNet2010\\NXDO.Mixed\\nxdo.jacoste");
		
		//ltd
		String fname = "E:\\DotNet2012\\CSharp2Java\\NXDO.Mixed.V2015\\Tester\\RJavaX64\\bin\\Debug\\jforloaded.jar";
		JCSharp jf = new JCSharp(fname);
		jf.fillClasses("E:\\DotNet2012\\CSharp2Java\\nxdo.jacoste");
	}
	
}
