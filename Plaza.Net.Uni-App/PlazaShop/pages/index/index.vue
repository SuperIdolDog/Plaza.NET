<template>
  <view class="page-container">
    <!-- 定位胶囊按钮 -->
    <view
      class="capsule"
      @click="navigateToMap"
      :style="{
        top: capsuleTop + 'px',
        height: capsuleHeight + 'px',
        borderRadius: (capsuleHeight / 2) + 'px'
      }"
    >
      <u-icon name="map-fill" color="#333" size="16" />
      <u-text
        :text="selectedPlazaName"
        color="#333"
        size="14"
        lines="1"
        customStyle="margin:0 10rpx;max-width:200rpx;white-space:nowrap;overflow:hidden;text-overflow:ellipsis;"
      />
      <u-icon name="arrow-right" color="#999" size="14" />
    </view>

    <!-- 其余内容保持不动 -->
    <view class="container">
      <view class="banner">
        <u-swiper
          :list="imgList"
          height="40vh"
          indicator
          indicatorActiveColor="#5555ff"
          indicatorMode="dot"
          circular
          autoplay
          duration="500"
          imgMode="HeightFix"
        />
      </view>

      <view class="notice-bar">
        <u-icon name="volume" color="#e6a23c" size="16" />
        <u-text
          text="今日商场营业时间：10:00-22:00，停车场免费时段：22:00-次日10:00"
          color="#e6a23c"
          size="13"
          customStyle="margin-left:10rpx;flex:1;"
        />
      </view>

      <view class="category-section">
        <view class="section-title">
          <u-text text="快捷助手" color="#5D4037" size="16" block />
          <u-line
            color="black"
            length="150"
            direction="row"
            customStyle="flex:1;height:4rpx;border-radius:2rpx;"
          />
        </view>
        <u-scroll-list style="margin-top:24rpx;">
          <view
            v-for="(item, index) in categories"
            :key="index"
            class="scroll-item"
            @click="handleCategoryClick(item)"
          >
            <view class="icon-wrapper">
              <u-icon :name="item.icon" :color="item.color" size="36" />
            </view>
            <u-text
              :text="item.name"
              color="#666"
              size="13"
              block
              align="center"
              customStyle="margin-top:16rpx;"
            />
          </view>
        </u-scroll-list>
      </view>

      <view class="activity-section">
        <view class="section-title">
          <u-text text="活动专区" color="#5D4037" size="16" block />
          <u-line
            color="black"
            length="150"
            direction="row"
            customStyle="flex:1;height:4rpx;border-radius:2rpx;"
          />
        </view>
        <view class="activity-carousel">
          <u-swiper
            :list="activityList"
            keyName="image"
            imgMode="easeInOutCubic"
            height="320rpx"
            previousMargin="30"
            nextMargin="30"
            radius="16"
            :autoplay="true"
            :circular="true"
            @click="handleActivityClick"
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
      selectedPlazaName: '', // 动态显示
      capsuleTop: 0,
      capsuleHeight: 32,
      imgList: [
        '/static/banner/banner@2x.png',
        '/static/banner/dpbanner@2x.png',
        '/static/banner/ffe67f59-ac5f-4757-9e84-84e9298170d7.jpg',
        '/static/banner/ff191f6e-8842-49d5-a38f-8299fd665ce6.jpg'
      ],
      categories: [
        { name: '停车服务', icon: 'car-fill', color: '#36C5FF', path: '/pages/park/park' },
        { name: '敬请期待', icon: 'clock-fill', color: '#FFAA33', path: '/pages/coupons/index' },
        { name: '敬请期待', icon: 'clock-fill', color: '#FFAA33', path: '/pages/coupons/index' },
        { name: '敬请期待', icon: 'clock-fill', color: '#FFAA33', path: '/pages/coupons/index' }
      ],
      activityList: [
        {
          id: 1,
          image: '/static/activity/uview2.png',
          title: '夏日清凉大促',
          description: '全场饮品第二杯半价',
          time: '2023.07.01-07.31',
          tag: '进行中',
          tagColor: '#FF6B6B'
        },
        {
          id: 2,
          image: '/static/activity/lightyearadmin.png',
          title: '美食狂欢节',
          description: '指定餐厅满100减30',
          time: '2023.07.15-07.20',
          tag: '即将开始',
          tagColor: '#4ECDC4'
        },
        {
          id: 3,
          image: '/static/swiper-background-3.png',
          title: '美食狂欢节',
          description: '指定餐厅满100减30',
          time: '2023.07.15-07.20',
          tag: '即将开始',
          tagColor: '#4ECDC4'
        }
      ]
    };
  },
  methods: {
    navigateToMap() {
      uni.navigateTo({ url: '/pages/map/mapTest' });
    },
    handleActivityClick(index) {
      const activity = this.activityList[index];
      uni.navigateTo({ url: `/pages/activity/detail?id=${activity.id}` });
    },
    handleCategoryClick(item) {
      if (item.path) uni.navigateTo({ url: item.path });
    }
  },
  onShow() {
    // 广场列表返回时通过全局事件拿 plazaName
    uni.$once('plazaSelected', (plaza) => {
      this.selectedPlazaName = plaza.name;
      uni.setStorageSync('selectedPlazaName', plaza.name); // 可持久化
    });
  },
  onLoad() {
    // 从缓存恢复
    this.selectedPlazaName =
      uni.getStorageSync('selectedPlazaName') || '';

    // 胶囊按钮位置
    try {
      // #ifdef MP-WEIXIN
      const info = uni.getMenuButtonBoundingClientRect();
      this.capsuleTop = info.top;
      this.capsuleHeight = info.height;
      // #endif
      // #ifdef H5
      const system = uni.getSystemInfoSync();
      this.capsuleTop = system.statusBarHeight || 10;
      // #endif
    } catch {
      this.capsuleTop = 10;
      this.capsuleHeight = 32;
    }
  }
};
</script>

<style lang="scss" scoped>
	/* 保留第二段核心样式，新增通知条样式 */
	.page-container {
		position: relative;
		width: 100%;
		min-height: 100vh;
		background-color: #FCFAF7;
	}
/* 横向滚动卡片 */
.scroll-item {
  display: inline-flex;
  flex-direction: column;
  align-items: center;
  padding: 0 40rpx;
  box-sizing: border-box;
  white-space: nowrap;   /* 防止文字换行 */
}
	.capsule {
		position: fixed;
		left: 20rpx;
		z-index: 999;
		display: flex;
		align-items: center;
		padding: 0 24rpx;
		background-color: rgba(255, 255, 255, 0.8);
		box-shadow: 0 2rpx 10rpx rgba(0, 0, 0, 0.05);
		backdrop-filter: blur(10px);

		&:active {
			opacity: 0.8;
			transform: scale(0.98);
		}

		.capsule-text {
			margin: 0 10rpx;
			font-size: 14px;
			color: #333;
			max-width: 200rpx;
			white-space: nowrap;
			overflow: hidden;
			text-overflow: ellipsis;
		}
	}

	/* 新增通知条样式 */
	.notice-bar {
		background: rgba(255, 245, 230, 0.9);
		margin: 20rpx 30rpx;
		padding: 20rpx;
		border-radius: 8rpx;
		display: flex;
		align-items: center;

		.notice-text {
			font-size: 13px;
			color: #e6a23c;
			margin-left: 10rpx;
			flex: 1;
		}
	}

	/* 调整活动卡片阴影效果 */
	.activity-card {
		box-shadow: 0 4rpx 16rpx rgba(0, 0, 0, 0.08);
		transition: transform 0.2s;

		&:active {
			transform: translateY(2rpx);
		}
	}

	/* 保持其他核心样式不变 */
	.banner {
		border-radius: 0 0 30rpx 30rpx;
		overflow: hidden;
		margin-bottom: 40rpx;
	}

	.container {
		padding-bottom: 120rpx;
	}

	.section-title {
		display: flex;
		align-items: center;
		margin: 40rpx 0;
		padding: 0 30rpx;

		.title-text {
			font-size: 32rpx;
			color: #5D4037;
			font-weight: 500;
			padding-right: 20rpx;
		}

		.title-line {
			flex: 1;
			height: 1rpx;
			background-color: #D7CCC8;
		}
	}

	.category-section {
		margin: 0 0 60rpx;

		.category-name {
			display: block;
			margin-top: 16rpx;
			font-size: 26rpx;
			color: #666;
			text-align: center;
		}
	}

	.activity-section {
		margin: 40rpx 0;
		padding: 0 30rpx;
	}

	.activity-carousel {
		// height: 400rpx; 
		margin-top: 20rpx;
	}

	.card-content {
		padding: 20rpx;
	}

	.activity-title {
		font-size: 30rpx;
		font-weight: 500;
		color: #333;
		display: block;
		margin-bottom: 10rpx;
	}

	.activity-desc {
		font-size: 24rpx;
		color: #666;
		display: block;
		margin-bottom: 20rpx;
		line-height: 1.4;
	}

	.activity-footer {
		display: flex;
		justify-content: space-between;
		align-items: center;
	}

	.activity-time {
		font-size: 22rpx;
		color: #999;
	}

	.tag {
		font-size: 20rpx;
		color: white;
		padding: 4rpx 12rpx;
		border-radius: 20rpx;
	}

	.icon-wrapper {
		width: 70px;
		height: 70px;
		border-radius: 50%;
		background: #fff;
		box-shadow: 0 4rpx 12rpx rgba(0, 0, 0, 0.06);
		display: flex;
		align-items: center;
		justify-content: center;
	}
</style>