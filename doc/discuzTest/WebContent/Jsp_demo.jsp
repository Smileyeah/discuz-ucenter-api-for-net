<%
/**
 * ================================================
 * Discuz! Ucenter API for JAVA
 * ================================================
 * JSP µ÷ÓÃÊ¾Àý
 * 
 * ¸ü¶àÐÅÏ¢£ºhttp://code.google.com/p/discuz-ucenter-api-for-java/
 * ×÷Õß£ºÁºÆ½ (no_ten@163.com) 
 * ´´½¨Ê±¼ä£º2009-2-20
 */
%>
<%@ page language="java" pageEncoding="UTF-8"%>
<%@page import="java.util.LinkedList"%>
<%@page import="java.net.URLEncoder"%>
<%@page import="com.fivestars.interfaces.bbs.util.XMLHelper"%>
<%@page import="com.fivestars.interfaces.bbs.client.Client"%>
<!DOCTYPE html>
<html lang="zh-cn">
<head>
<title>test</title>
</head>
<body>
<%
Client uc = new Client();
/* String $ucsynlogout = uc.uc_user_synlogout();
System.out.println("退出成功" + $ucsynlogout);
out.println($ucsynlogout); */
String result = uc.uc_user_login("郭金明", "123456");

LinkedList<String> rs = XMLHelper.uc_unserialize(result);
if(rs.size()>0){
	int $uid = Integer.parseInt(rs.get(0));
	String $username = rs.get(1);
	String $password = rs.get(2);
	String $email = rs.get(3);
	if($uid > 0) {
		response.addHeader("P3P"," CP=\"CURa ADMa DEVa PSAo PSDo OUR BUS UNI PUR INT DEM STA PRE COM NAV OTC NOI DSP COR\"");

		out.println("登录成功");
		out.println($username);
		out.println($password);
		out.println($email);
		
		String $ucsynlogin = uc.uc_user_synlogin($uid);
		out.println("登录成功"+$ucsynlogin);

		//±¾µØµÇÂ½´úÂë
		//TODO ... ....
		
		Cookie auth = new Cookie("auth", uc.uc_authcode($password+"\t"+$uid, "ENCODE"));
		auth.setMaxAge(31536000);
		//auth.setDomain("localhost");
		response.addCookie(auth);
		
		Cookie user = new Cookie("uchome_loginuser", URLEncoder.encode($username, "utf-8"));
		response.addCookie(user);
		
	} else if($uid == -1) {
		out.println("用户不存在,或者被删除");
	} else if($uid == -2) {
		out.println("密码错");
	} else {
		out.println("未定义");
	}
}else{
	out.println("Login failed");
	System.out.println(result);
}
%>
</body>
</html>