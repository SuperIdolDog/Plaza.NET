<template>
  <view class="page">
    <view class="header-glass">
      <view class="avatar-wrapper">
        <image class="avatar" :src="user.avatar" @click="chooseAvatar" />
        <view class="camera" @click="chooseAvatar"><u-icon name="camera-fill" color="#fff" size="14" /></view>
      </view>
      <view class="user-info">
        <text class="nick">{{ user.nick || '亲爱的用户' }}</text>
        <text class="meta">账号 {{ user.account }}</text>
        <text class="meta">入会时间 {{ user.joinTime }}</text>
      </view>
    </view>

    <view class="info-list">
      <view v-for="(item, i) in list" :key="i" class="cell" @click="item.handler">
        <text class="label">{{ item.label }}</text>
        <view class="value-area">
          <text class="value">{{ item.value }}</text>
          <u-icon v-if="item.arrow" name="arrow-right" size="14" color="#c0c0c0" />
        </view>
      </view>
    </view>

    <view class="tip">性别信息、生日信息涉及权益发放，请谨慎修改</view>

    <view class="save-box">
      <button class="save-btn" @click="save">保存</button>
    </view>

    <u-action-sheet :show="showGender" :actions="genderActions" @close="showGender=false" @select="selectGender" />
    <u-datetime-picker :show="showBirth" v-model="pickerBirth" mode="date" @confirm="confirmBirth" @cancel="showBirth=false" />
  </view>
</template>

<script>
export default {
  data() {
    const info = uni.getStorageSync('userInfo') || {};
    return {
      user: {
        avatar   : info.avatar || '/static/logo.png',
        nick     : info.userName || '',
        account  : info.userName || '',
        phone    : info.phoneNumber || '未绑定',
        gender   : 0,
        genderText:'未知',
        birth    : '',
        joinTime : info.registerDate ? info.registerDate.substring(0,10) : ''
      },
      genderActions: [{ name: '男', value: 1 }, { name: '女', value: 2 }, { name: '未知', value: 0 }],
      showGender: false,
      showBirth: false,
      pickerBirth: Date.now()
    };
  },
  computed: {
    list() {
      return [
        { label: '性别', value: this.user.genderText, arrow: true, handler: () => (this.showGender = true) },
        { label: '手机号', value: this.user.phone, arrow: true, handler: () => uni.showToast({title:'暂未开放',icon:'none'}) },
        { label: '生日', value: this.user.birth || '完善生日，享生日好礼 >', arrow: true, handler: () => (this.showBirth = true) }
      ];
    }
  },
  methods: {
    chooseAvatar() {
      uni.chooseImage({ count: 1, success: res => this.user.avatar = res.tempFilePaths[0] });
    },
    selectGender(e) {
      this.user.gender = e.value;
      this.user.genderText = e.name;
      this.showGender = false;
    },
    confirmBirth(e) {
      this.user.birth = uni.$u.timeFormat(e.value, 'yyyy-mm-dd');
      this.showBirth = false;
    },
    save() {
      uni.showLoading({ title: '保存中...' });
      // 上传头像/昵称到服务器... 这里先演示本地缓存
      const latest = { ...uni.getStorageSync('userInfo'), avatar: this.user.avatar, userName: this.user.nick };
      uni.setStorageSync('userInfo', latest);
      uni.hideLoading();
      uni.showToast({ title: '保存成功', icon: 'success' });
      // 通知个人中心更新
      uni.$emit('user:info', latest);
    }
  }
};
</script>

<style lang="scss" scoped>
page { background: #faf8f5; padding-bottom: 60rpx; }
.header-glass { margin: 24rpx 32rpx 40rpx; height: 240rpx; background: linear-gradient(135deg,#c7a47e 0%,#b8956d 100%); border-radius: 28rpx; display: flex; align-items: center; padding: 0 40rpx; position: relative; overflow: hidden; box-shadow: 0 8rpx 28rpx rgba(199,164,126,.12); }
.avatar-wrapper { position: relative; z-index: 1; }
.avatar { width: 120rpx; height: 120rpx; border-radius: 50%; border: 4rpx solid rgba(#fff,.9); }
.camera { position: absolute; right: -4rpx; bottom: 0; width: 36rpx; height: 36rpx; background: #c7a47e; border: 2rpx solid #fff; border-radius: 50%; display: flex; align-items: center; justify-content: center; }
.user-info { margin-left: 30rpx; display: flex; flex-direction: column; justify-content: center; z-index: 1; }
.nick { font-size: 36rpx; font-weight: 600; color: #fff; }
.meta { font-size: 26rpx; color: rgba(#fff,.85); margin-top: 4rpx; }
.info-list { margin: 0 32rpx; background: #fff; border-radius: 28rpx; box-shadow: 0 8rpx 28rpx rgba(199,164,126,.12); overflow: hidden; }
.cell { display: flex; align-items: center; padding: 34rpx 32rpx; font-size: 30rpx; color: #333; border-bottom: 1rpx solid #f5f5f5; &:last-child { border-bottom: none; } }
.label { width: 180rpx; font-weight: 500; }
.value-area { flex: 1; display: flex; align-items: center; justify-content: flex-end; }
.value { color: #666; margin-right: 8rpx; }
.tip { margin: 24rpx 32rpx 0; font-size: 24rpx; color: #999; text-align: center; }
.save-box { margin: 60rpx 32rpx 0; }
.save-btn { width: 100%; height: 88rpx; line-height: 88rpx; background: linear-gradient(135deg,#c7a47e 0%,#b8956d 100%); border-radius: 44rpx; color: #fff; font-size: 32rpx; box-shadow: 0 6rpx 20rpx rgba(#b8956d,.3); &:active { transform: scale(.96); } }
</style>