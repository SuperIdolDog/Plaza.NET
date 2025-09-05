<template>
  <view class="content">
    <!-- 胶囊返回按钮 -->
    <view
      class="back-capsule"
      :style="{
        top: capsuleTop + 'px',
        height: capsuleHeight + 'px',
        borderRadius: capsuleHeight / 2 + 'px'
      }"
      @click="goBack"
    >
      <u-icon name="arrow-left" color="#fff" size="16" />
    </view>

    <!-- 登录卡片 -->
    <view class="login-card">
      <!-- 切换 Tab -->
      <view class="tab-bar">
        <view
          class="tab-item"
          :class="{ active: loginType === 'phone' }"
          @click="loginType = 'phone'"
        >
          手机验证码登录
        </view>
        <view
          class="tab-item"
          :class="{ active: loginType === 'account' }"
          @click="loginType = 'account'"
        >
          账号密码登录
        </view>
      </view>

      <!-- 手机验证码登录 -->
      <u-form
        v-show="loginType === 'phone'"
        ref="phoneForm"
        :model="phoneForm"
        :rules="phoneRules"
        label-position="top"
      >
        <u-form-item prop="phone">
          <u-input
            v-model="phoneForm.phone"
            placeholder="请输入手机号"
            prefix-icon="phone"
            prefix-icon-style="font-size: 40rpx; color: #42a1fa"
            border="surround"
            clearable
          />
        </u-form-item>

        <u-form-item prop="code">
          <u-input
            v-model="phoneForm.code"
            placeholder="请输入验证码"
            prefix-icon="checkmark"
            prefix-icon-style="font-size: 40rpx; color: #42a1fa"
            border="surround"
            clearable
          >
            <template #suffix>
              <u-button
                v-if="!phoneCodeTimer"
                type="success"
                size="mini"
                @click="sendPhoneCode"
              >
                获取验证码
              </u-button>
              <u-button v-else type="default" size="mini" disabled>
                {{ phoneCodeTime }}s后重试
              </u-button>
            </template>
          </u-input>
        </u-form-item>

        <u-button
          type="primary"
          size="large"
          :disabled="!phoneForm.phone || !phoneForm.code"
          @click="submitPhone"
        >
          登录
        </u-button>
      </u-form>

      <!-- 账号密码登录 -->
      <u-form
        v-show="loginType === 'account'"
        ref="accountForm"
        :model="accountForm"
        :rules="accountRules"
        label-position="top"
      >
        <u-form-item prop="username">
          <u-input
            v-model="accountForm.username"
            placeholder="请输入账号/邮箱"
            prefix-icon="account"
            prefix-icon-style="font-size: 40rpx; color: #42a1fa"
            border="surround"
            clearable
          />
        </u-form-item>

        <u-form-item prop="password">
          <u-input
            v-model="accountForm.password"
            type="password"
            placeholder="请输入密码"
            prefix-icon="lock"
            prefix-icon-style="font-size: 40rpx; color: #42a1fa"
            border="surround"
            clearable
          />
        </u-form-item>

        <u-button
          type="primary"
          size="large"
          :disabled="!accountForm.username || !accountForm.password"
          @click="submitAccount"
        >
          登录
        </u-button>
      </u-form>

      <!-- 第三方登录 -->
      <view class="other-login">
        <u-divider text="其他账号登录" text-color="#cbcbcb" />
        <view class="icon-row">
          <u-icon
            name="weixin-circle-fill"
            size="80rpx"
            color="#07c160"
            @tap="loginWeixin"
          />
          <u-icon
            name="qq-circle-fill"
            size="80rpx"
            color="#00aaff"
            @tap="loginQQ"
          />
        </view>
      </view>
    </view>
  </view>
</template>

<script>
export default {
  data() {
    return {
      loginType: 'phone',
      phoneForm: { phone: '', code: '' },
      phoneRules: {
        phone: [
          { required: true, message: '请输入手机号', trigger: 'blur' },
          { pattern: /^1[3-9]\d{9}$/, message: '手机号格式不正确', trigger: 'blur' }
        ],
        code: [{ required: true, message: '请输入验证码', trigger: 'blur' }]
      },
      phoneCodeTimer: null,
      phoneCodeTime: 60,
      accountForm: { username: 'SuperIdol', password: 'Access123!@#' },
      accountRules: {
        username: [{ required: true, message: '请输入账号', trigger: 'blur' }],
        password: [{ required: true, message: '请输入密码', trigger: 'blur' }]
      },
      capsuleTop: 0,
      capsuleHeight: 32
    };
  },
  onLoad() {
    /* 胶囊按钮定位 */
    // #ifdef MP-WEIXIN
    const info = uni.getMenuButtonBoundingClientRect();
    this.capsuleTop = info.top;
    this.capsuleHeight = info.height;
    // #endif
    // #ifdef H5
    const sys = uni.getSystemInfoSync();
    this.capsuleTop = sys.statusBarHeight || 10;
    this.capsuleHeight = 32;
    // #endif
  },
  onUnload() {
    if (this.phoneCodeTimer) clearInterval(this.phoneCodeTimer);
  },
  methods: {
    /* 发送验证码（仅演示倒计时） */
    sendPhoneCode() {
      this.$refs.phoneForm.validateField('phone').then(() => {
        if (this.phoneCodeTimer) return;
        this.phoneCodeTimer = setInterval(() => {
          this.phoneCodeTime -= 1;
          if (this.phoneCodeTime <= 0) {
            clearInterval(this.phoneCodeTimer);
            this.phoneCodeTimer = null;
            this.phoneCodeTime = 60;
          }
        }, 1000);
      });
    },

    /* 手机验证码登录（示例） */
    submitPhone() {
      this.$refs.phoneForm.validate(async valid => {
        if (!valid) return;
        uni.showLoading({ title: '登录中' });
        /* 模拟成功 */
        const user = {
          avatar: 'https://dummyimage.com/200x200/42a1fa/fff  ',
          nickName: '手机用户',
          levelName: '黄金会员'
        };
        uni.setStorageSync('token', 'fake_token');
        uni.setStorageSync('userInfo', user);
        uni.$emit('user:info', user);
        uni.showToast({ title: '登录成功', icon: 'success' });
        setTimeout(() => uni.switchTab({ url: '/pages/personal/personalTest' }), 800);
      });
    },

    /* 账号密码登录 */
    async submitAccount() {
      const valid = await this.$refs.accountForm.validate();
      if (!valid) return;
      uni.showLoading({ title: '登录中' });
      try {
        const res = await uni.request({
          url: '/api/user/login',
          method: 'POST',
          data: {
            account: this.accountForm.username,
            password: this.accountForm.password
          }
        });
        const { data } = res;
        if (data.success && data.token) {
          const user = {
            account:  data.userInfo.account  || '用户'
          };
          uni.setStorageSync('token', data.token);
          uni.setStorageSync('userInfo', user);
          uni.$emit('user:info', user);
          uni.showToast({ title: '登录成功', icon: 'success' });
          setTimeout(() => uni.switchTab({ url: '/pages/personal/personalTest' }), 800);
        } else {
          throw new Error(data.message || '登录失败');
        }
      } catch (e) {
		  console.log(e.message);
        uni.showToast({ title: e.errMsg || e.message || '网络异常', icon: 'none' });
		
      } finally {
        uni.hideLoading();
      }
    },

    /* 微信登录（示例） */
    async loginWeixin() {
      // #ifdef MP-WEIXIN
      try {
        const { userInfo } = await this.getUserProfile();
        const { code } = await wx.login();
        uni.showLoading({ title: '登录中…' });
        /* 模拟返回 */
        const user = {
          
          userName: userInfo.userName,

        };
        uni.setStorageSync('token', 'fake_token');
        uni.setStorageSync('userInfo', user);
        uni.$emit('user:info', user);
        uni.showToast({ title: '登录成功', icon: 'success' });
        setTimeout(() => uni.switchTab({ url: '/pages/personal/personalTest' }), 800);
      } catch (e) {
        uni.showToast({ title: e.errMsg || e.message || '登录失败', icon: 'none' });
      }
      // #endif
      // #ifndef MP-WEIXIN
      uni.showToast({ title: '请在微信小程序中使用', icon: 'none' });
      // #endif
    },

    loginQQ() {
      uni.showToast({ title: 'QQ 登录暂未开放', icon: 'none' });
    },

    getUserProfile() {
      return new Promise((resolve, reject) => {
        wx.getUserProfile({ desc: '用于完善会员资料', success: resolve, fail: reject });
      });
    },

    goBack() {
      uni.navigateBack({ delta: 1 });
    }
  }
};
</script>

<style lang="scss" scoped>
.content {
  height: 100vh;
  position: relative;
}
.back-capsule {
  position: fixed;
  left: 20rpx;
  z-index: 999;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0 24rpx;
  background-color: rgba(85, 170, 255, 0.2);
  backdrop-filter: blur(10px);
  box-shadow: 0 2rpx 10rpx rgba(0, 0, 0, 0.05);
}
.login-card {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -60%);
  width: 90%;
  padding: 60rpx;
  background: #fff;
  border-radius: 20rpx;
  box-sizing: border-box;
}
.tab-bar {
  display: flex;
  margin-bottom: 60rpx;
}
.tab-item {
  flex: 1;
  text-align: center;
  font-size: 32rpx;
  color: #666;
  position: relative;
}
.tab-item.active {
  color: #42a1fa;
  font-weight: 600;
}
.tab-item.active::after {
  content: '';
  position: absolute;
  left: 50%;
  bottom: -12rpx;
  transform: translateX(-50%);
  width: 40rpx;
  height: 4rpx;
  background: #42a1fa;
  border-radius: 2rpx;
}
.other-login {
  margin-top: 100rpx;
}
.icon-row {
  margin-top: 30rpx;
  display: flex;
  justify-content: center;
  gap: 60rpx;
}
</style>