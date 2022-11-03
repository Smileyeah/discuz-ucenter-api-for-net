<?php
/**
 * UCenter Ӧ�ó��򿪷� Example
 *
 * �г����ѵ� Example ����
 * ʹ�õ��Ľӿں�����
 * uc_friend_totalnum()	���룬���غ�������
 * uc_friend_ls()	���룬���غ����б�
 * uc_friend_delete()	���룬ɾ������
 * uc_friend_add()	���룬���Ӻ���
 */

if(empty($_POST['submit'])) {
	$num = uc_friend_totalnum($Example_uid);
	echo '���� '.$num.' ������';
	echo '<form method="post" action="'.$_SERVER['PHP_SELF'].'?example=friend">';
	$friendlist = uc_friend_ls($Example_uid, 1, 999, $num);
	if($friendlist) {
		foreach($friendlist as $friend) {
			echo '<input type="checkbox" name="delete[]" value="'.$friend['friendid'].'">';
			switch($friend['direction']) {
				case 1: echo '[��ע]';break;
				case 3: echo '[����]';break;
			}
			echo $friend['username'].':'.$friend['comment'].'<br>';
		}
	}
	echo '���Ӻ���:<input name="newfriend"> ˵��:<input name="newcomment"><br>';
	echo '<input name="submit" type="submit"> ';
	echo '</form>';
} else {
	if(!empty($_POST['delete']) && is_array($_POST['delete'])) {
		uc_friend_delete($Example_uid, $_POST['delete']);
	}
	if($_POST['newfriend'] && $friendid = uc_get_user($_POST['newfriend'])) {
		uc_friend_add($Example_uid, $friendid[0], $_POST['newcomment']);
	}
	echo '���������Ѹ���<br><a href="'.$_SERVER['PHP_SELF'].'?example=friend">����</a>';
	exit;
}


?>