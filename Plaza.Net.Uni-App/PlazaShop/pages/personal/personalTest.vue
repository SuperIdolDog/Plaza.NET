<template>
	<view class="page">
		<view class="header-bg">
			<image class="bg-img" src="/static/swiper-background-1.png" mode="aspectFill" />
		</view>

		<!-- 用户信息 -->
		<view class="user-card">
			<view v-if="!isLogin" class="user-unlogin" @tap="toLogin">
				<u-avatar src="/static/logo.png" size="60" />
				<u-text text="点击登录 / 注册" color="#333" size="16" />
			</view>
			<view v-else class="user-logined">
				<view class="avatar-box">
					<u-avatar :src="avatar" size="60" />
					
				</view>
				<view class="info-box">
					<u-text :text="userName" color="#333" size="16" bold />
				</view>
				<u-icon name="arrow-right" size="12" color="#333" @tap="toProfile" />
			</view>
		</view>

		<!-- 资产 -->
		<view class="asset-wrap">
			<view class="asset-pill" @tap="toAsset('balance')">
				<u-text :text="balance" color="black" size="20" bold align="center" />
				<u-text text="余额" color="black" size="12" align="center" />
			</view>
			<view class="asset-pill" @tap="toAsset('collect')">
				<u-text :text="collect" color="black" size="20" bold align="center" />
				<u-text text="收藏" color="black" size="12" align="center" />
			</view>
			<view class="asset-pill" @tap="toAsset('footprint')">
				<u-text :text="footprint" color="black" size="20" bold align="center" />
				<u-text text="足迹" color="black" size="12" align="center" />
			</view>
		</view>

		<!-- 功能入口 -->
		<view class="menu-wrap">
			<view v-for="item in menuList" :key="item.title" class="menu-item" @tap="item.click">
				<u-icon :name="item.icon" size="26" color="black" />
				<u-text :text="item.title" color="#3d2e1e" size="15" bold />
				<u-icon name="arrow-right" size="14" color="#999" />
			</view>
			<view v-if="isLogin" class="menu-item logout" @tap="logout">
				<u-icon name="close-circle" size="26" color="#e64340" />
				<u-text text="退出登录" color="#e64340" size="15" bold />
				<u-icon name="arrow-right" size="14" color="#999" />
			</view>
		</view>
	</view>
</template>

<script>
	export default {
		data() {
			return {
				isLogin: false,
				id: 0,
				userName: '',
				avatar: '/static/logo.png',
				balance: 0,
				collect: 0,
				footprint: 0,
				menuList: [{
						title: '个人信息',
						icon: 'account',
						click: () => this.toProfile()
					},
					{
						title: '地址管理',
						icon: 'map',
						click: () => this.toAddress()
					},
					{
						title: '用户反馈',
						icon: 'edit-pen',
						click: () => this.toFeedback()
					},
					{
						title: '我的消息',
						icon: 'bell',
						click: () => this.toEmail()
					},
					{
						title: '分享有礼',
						icon: 'share',
						click: () => this.toShare()
					}
				]
			};
		},
		onShow() {
			const cached = uni.getStorageSync('userInfo') || {};
			this.id = cached.id || 0;
			this.userName = cached.userName || '';
			this.avatar = cached.avatar || '/static/logo.png';
			this.balance = cached.balance || 0;
			this.collect = cached.collect || 0;
			this.footprint = cached.footprint || 0;
			this.isLogin = !!cached.userName;

			const token = uni.getStorageSync('token');
			if (!token) return;

			uni.request({
			  url: '/api/User/me',
			  method: 'GET',
			  header: { Authorization: `Bearer ${token}` },
			  success: (res) => {
			    if (res.data.success) {
			      const u = res.data.data;
			      const latest = {
			        id: u.id,
			        userName: u.userName,
			        avatar: u.avatarUrl,
			        balance: u.balance,
			        collect: u.collect,
			        footprint: u.footprint
			      };
			      uni.setStorageSync('userInfo', latest);
			      Object.assign(this, latest);
			      this.isLogin = true;
			    }
			  },
			  fail: () => uni.showToast({ title: '获取用户信息失败', icon: 'none' })
			});
			
		},
		methods: {
			toLogin() {
				uni.navigateTo({
					url: '/pages/index/login'
				});
			},
			toProfile() {
				uni.navigateTo({
					url: `/pages/personal/profile?userId=${this.id}`
				});
			},
			toAddress() {
				uni.navigateTo({
					url: `/pages/personal/address?userId=${this.id}`
				});
			},
			toFeedback() {
				uni.navigateTo({
					url: `/pages/personal/feedback?userId=${this.id}`
				});
			},
			toEmail() {
				uni.showToast({ title: '功能暂未开放', icon: 'none' });
				uni.navigateTo({
					url: '/pages/email/index'
				});
			},
			toShare() {
				uni.showToast({ title: '功能暂未开放', icon: 'none' });
				uni.navigateTo({
					url: '/pages/share/index'
				});
			},
			toAsset(type) {
				if (!this.isLogin) return uni.showToast({
					title: '请先登录',
					icon: 'none'
				});
				uni.showToast({ title: '功能暂未开放', icon: 'none' });
			},
			logout() {
			  uni.showModal({
			    title: '提示',
			    content: '确定退出登录？',
			    success: (res) => {
			      if (res.confirm) {
			        uni.request({
			          url: '/api/User/logout', // 注意这里改成 logout 接口
			          method: 'GET',
			          header: { Authorization: `Bearer ${uni.getStorageSync('token')}` }, // 假设需要认证头
			          success: (response) => {
			            if (response.data.success) {
			              uni.removeStorageSync('token');
			              uni.removeStorageSync('userInfo');
			              this.isLogin = false;
			              Object.assign(this, {
			                id: 0,
			                userName: '',
			                avatar: '/static/logo.png',
			                balance: 0,
			                collect: 0,
			                footprint: 0
			              });
			              uni.showToast({ title: '已退出登录', icon: 'success' });
			            } else {
			              uni.showToast({ title: '退出失败', icon: 'none' });
			            }
			          },
			          fail: () => {
			            uni.showToast({ title: '请求失败，请稍后再试', icon: 'none' });
			          }
			        });
			      }
			    }
			  });
			}
		}
	};
</script>

<style lang="scss" scoped>
	page {
		background: #f5f5f5;
	}

	.header-bg {
		position: relative;
		width: 100%;
		height: 250px;
		overflow: hidden;
	}

	.bg-img {
		width: 100%;
		height: 100%;
	}

	.user-card {
		position: relative;
		margin: -40px 15px 15px;
		padding: 35px 15px 25px;
		background: #fff;
		border-radius: 10px;
		box-shadow: 0 4px 16px rgba(0, 0, 0, .08);
	}

	.user-unlogin,
	.user-logined {
		display: flex;
		align-items: center;
	}

	.avatar-box {
		position: relative;
	}

	.vip-badge {
		position: absolute;
		bottom: 0;
		right: 0;
		width: 16px;
		height: 16px;
		border-radius: 50%;
		background: #fff;
		padding: 2px;

		image {
			width: 100%;
			height: 100%;
		}
	}

	.info-box {
		flex: 1;
		margin-left: 15px;
	}

	.asset-wrap {
		margin: 24rpx 30rpx;
		display: flex;
		gap: 20rpx;
	}

	.asset-pill {
		flex: 1;
		padding: 28rpx 0;
		background: #fff;
		border-radius: 20rpx;
		box-shadow: 0 6rpx 20rpx rgba(0, 0, 0, .1);
		display: flex;
		flex-direction: column;
		align-items: center;
	}

	.menu-wrap {
		margin: 0 30rpx 40rpx;
		background: #fff;
		border-radius: 20rpx;
		box-shadow: 0 6rpx 20rpx rgba(0, 0, 0, .05);
	}

	.menu-item {
		padding: 30rpx;
		display: flex;
		align-items: center;
		justify-content: space-between;
		border-bottom: 1rpx solid #f5f5f5;

		&:last-child {
			border-bottom: none;
		}
	}

	.logout {
		margin-top: 20rpx;
	}
</style>