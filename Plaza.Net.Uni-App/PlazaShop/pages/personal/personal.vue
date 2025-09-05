<template>
  <view class="page">

  </view>
</template>

<script>
export default {

  methods: {
    goLogin() {
      this.$refs.auth.open();
    },
    onLoginSuccess() {
      // 登录成功后刷新个人资料
      this.isLogin = true;
    }
  }
};
</script>