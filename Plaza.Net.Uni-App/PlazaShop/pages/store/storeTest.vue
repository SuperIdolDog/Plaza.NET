<template>
  <view class="container">
    <!-- 顶部搜索 -->
    <view class="header" id="header">
      <u-search
        v-model="searchKeyword"
        placeholder="请输入要搜索的店铺名"
        :show-action="false"
        :clearabled="true"
        bgColor="#f5f5f5"
        height="70rpx"
        @search="resetLoad"
        @clear="resetLoad"
      />
    </view>

    <view class="main-content">
      <!-- 左侧分类 -->
      <view class="category-sidebar">
        <view
          class="category-item"
          :class="{ active: currentCategory === item.id }"
          v-for="item in categories"
          :key="'c-' + item.id"
          @click="selectCategory(item)"
        >
          <u-text :text="item.name" align="center" />
        </view>
      </view>

      <!-- 右侧可滚动店铺列表 -->
      <view class="store-list-wrapper">
        <scroll-view
          class="store-scroll"
          scroll-y
          :style="{ height: scrollHeight }"
          @scrolltolower="onScrollToLower"
        >
          <view
            class="store-card"
            v-for="store in stores"
            :key="'s-' + store.id"
            @click="viewStore(store)"
          >
            <u-image
              :src="store.imageURL"
              class="store-logo-img"
              mode="aspectFit"
              width="100rpx"
              height="100rpx"
            />
            <view class="store-info">
              <u-text class="store-name" :text="store.name" />
              <u-text
              
                :text="'位置:' + store.location"
				prefix-icon="map-fill"
              />
              <u-text class="store-desc" :text="store.description" />
            </view>
          </view>

          <!-- 空态 -->
          <u-empty
            v-if="!stores.length && !isLoading"
            mode="list"
            marginTop="120rpx"
          />

          <!-- 加载更多 -->
          <u-loadmore
            v-if="stores.length"
            :status="loadStatus"
            loading-text="努力加载中..."
            loadmore-text="上拉加载更多"
            nomore-text="没有更多了"
            margin-top="30"
            margin-bottom="30"
          />
        </scroll-view>
      </view>
    </view>

    <!-- 右下角楼层导航 -->
    <view class="floor-nav">
      <view class="floor-nav-container">
        <view class="floor-nav-item arrow-item" @click="scrollTop">
          <u-icon name="arrow-up" size="20" color="#001733" />
        </view>
        <scroll-view class="floor-nav-scroll" scroll-y>
          <view
            class="floor-nav-item"
            :class="{ active: currentFloorId === floor.id }"
            v-for="floor in floors"
            :key="'f-' + floor.id"
            @click="selectFloor(floor)"
          >
            <u-text :text="floor.floorItemName" align="center" />
          </view>
        </scroll-view>
        <view class="floor-nav-item arrow-item" @click="scrollBottom">
          <u-icon name="arrow-down" size="20" color="#001733" />
        </view>
      </view>
    </view>
  </view>
</template>

<script>
export default {
  data() {
    return {
      searchKeyword: '',
      currentCategory: 'all',
      currentFloorId: 0,
      categories: [{ id: 'all', name: '全部' }],
      floors: [{ id: 0, floorItemName: '全部' }],
      stores: [],
      plazaId: null,

      scrollHeight: '100vh',

      /* 分页 & 状态 */
      pageIndex: 1,
      pageSize: 10,
      hasMore: true,
      loadStatus: 'loadmore',
      isLoading: false
    };
  },
  async onLoad() {
    const plaza = uni.getStorageSync('selectedPlaza');
    if (!plaza) {
      uni.showToast({ title: '请先选择广场', icon: 'none' });
      return uni.navigateTo({ url: '/pages/map/mapTest' });
    }
    this.plazaId = plaza.id;
    await Promise.all([this.loadFloors(), this.loadCategories()]);
    this.calcScrollHeight();
    this.resetLoad();
  },
  onShow() {
    const plaza = uni.getStorageSync('selectedPlaza');
    if (!plaza) {
      uni.showToast({ title: '请先选择广场', icon: 'none' });
      return uni.navigateTo({ url: '/pages/map/mapTest' });
    }
    if (plaza.id !== this.plazaId) {
      this.plazaId = plaza.id;
      this.searchKeyword = '';
      this.currentCategory = 'all';
      this.currentFloorId = 0;
      this.resetLoad();
    }
  },
  methods: {
    calcScrollHeight() {
      const query = uni.createSelectorQuery().in(this);
      query.select('#header').boundingClientRect();
      query.exec(res => {
        const headerH = res[0]?.height || 88;
        // 去掉 header + 底部 floor-nav 140rpx + 安全间隙
        this.scrollHeight = `calc(100vh - ${headerH}px - var(--window-top) - 140rpx)`;
      });
    },
    async loadFloors() {
      const res = await uni.request({ url: `/api/plaza/${this.plazaId}/floors` });
      this.floors = [{ id: 0, floorItemName: '全部' }, ...res.data];
    },
    async loadCategories() {
      const res = await uni.request({ url: '/api/store/storetypes' });
      this.categories = [
        { id: 'all', name: '全部' },
        ...res.data.map(t => ({ id: t.id.toString(), name: t.name }))
      ];
    },
    async loadStores(reset = false) {
      if (reset) {
        this.pageIndex = 1;
        this.hasMore = true;
        this.stores = [];
      }
      if (!this.hasMore) return;

      this.isLoading = true;
      this.loadStatus = 'loading';

      const params = {
        plazaId: this.plazaId,
        pageIndex: this.pageIndex,
        pageSize: this.pageSize
      };
      if (this.currentFloorId !== 0) params.floorId = this.currentFloorId;
      if (this.currentCategory !== 'all') params.storeTypeId = this.currentCategory;
      if (this.searchKeyword) params.keyword = this.searchKeyword;

      try {
        const res = await uni.request({ url: '/api/store/stores', data: params });
        const list = res.data || [];
        this.stores.push(...list);
        this.hasMore = list.length === this.pageSize;
        this.loadStatus = this.hasMore ? 'loadmore' : 'nomore';
      } finally {
        this.isLoading = false;
      }
    },
    onScrollToLower() {
      if (this.loadStatus !== 'loadmore') return;
      this.pageIndex += 1;
      this.loadStores();
    },
    selectCategory(item) {
      this.currentCategory = item.id;
      this.resetLoad();
    },
    selectFloor(floor) {
      this.currentFloorId = floor.id;
      this.resetLoad();
    },
    resetLoad() {
      this.loadStores(true);
    },
    viewStore(store) {
      uni.navigateTo({ url: `/pages/store/storedetailTest?id=${store.id}` });
    },
    scrollTop() {
      uni.pageScrollTo({ scrollTop: 0, duration: 300 });
    },
    scrollBottom() {
      uni.pageScrollTo({ scrollTop: 99999, duration: 300 });
    }
  }
};
</script>

<style lang="scss" scoped>

.container {
  height: 100vh;
  background: linear-gradient(135deg, #f0f4ff 0%, #ffffff 100%);
  display: flex;
  flex-direction: column;
}

.header {
  background: #fff;
  padding: 20rpx 30rpx;
  border-radius: 0 0 20rpx 20rpx;
  box-shadow: 0 4rpx 12rpx rgba(0, 0, 0, 0.05);
  border: 1rpx solid rgba(124, 58, 237, 0.1);
}

.main-content {
  flex: 1;
  display: flex;
  overflow: hidden;
  margin-top: 16rpx;
}

.category-sidebar {
  width: 180rpx;
  background: #fff;
  border-radius: 20rpx 0 0 20rpx;
  box-shadow: 0 4rpx 12rpx rgba(0, 0, 0, 0.05);
  margin-right: 8rpx;
  overflow: hidden;
}

.category-item {
  padding: 28rpx 0;
  text-align: center;
  font-size: 26rpx;
  color: #666;
  border-bottom: 1rpx solid rgba(124, 58, 237, 0.05);
  transition: all 0.3s ease;
}
.category-item:last-child {
  border-bottom: none;
}
.category-item.active {
  background: linear-gradient(135deg, #7c3aed 0%, #a855f7 100%);
  color: #fff;
  font-weight: 600;
  box-shadow: 0 4rpx 12rpx rgba(124, 58, 237, 0.3);
}
.category-item:active {
  transform: scale(0.95);
}

.store-list-wrapper {
  flex: 1;
  background: #fff;
  border-radius: 0 20rpx 20rpx 0;
  box-shadow: 0 4rpx 12rpx rgba(0, 0, 0, 0.05);
  overflow: hidden;
}

.store-scroll {
  padding: 20rpx;
}

.store-card {
  display: flex;
  align-items: center;
  background: #fff;
  border-radius: 16rpx;
  padding: 24rpx;
  margin-bottom: 20rpx;
  box-shadow: 0 4rpx 12rpx rgba(0, 0, 0, 0.05);
  transition: all 0.3s ease;
}
.store-card:last-child {
  margin-bottom: 0;
}
.store-card:active {
  transform: scale(0.98);
  box-shadow: 0 6rpx 16rpx rgba(0, 0, 0, 0.1);
}

.store-logo-img {
  width: 100rpx;
  height: 100rpx;
  border-radius: 12rpx;
  margin-right: 20rpx;
  box-shadow: 0 2rpx 8rpx rgba(0, 0, 0, 0.08);
}

.store-info {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 6rpx;
}

.store-name {
  font-size: 30rpx;
  font-weight: 600;
  color: #333;
  line-height: 1.4;
}

.store-location {
  font-size: 26rpx;
  color: #666;
  display: flex;
  align-items: center;
}
.store-location::before {
  content: '📍';
  margin-right: 4rpx;
}

.store-desc {
  font-size: 24rpx;
  color: #999;
  line-height: 1.5;
}

.floor-nav {
  position: fixed;
  right: 30rpx;
  bottom: 120rpx;
  z-index: 100;
}

.floor-nav-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 12rpx;
  background: rgba(255, 255, 255, 0.95);
  border-radius: 40rpx;
  padding: 16rpx;
  box-shadow: 0 6rpx 20rpx rgba(0, 0, 0, 0.1);
  backdrop-filter: blur(10px);
  border: 1rpx solid rgba(124, 58, 237, 0.1);
}

.floor-nav-scroll {
  max-height: 400rpx;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  gap: 8rpx;
}

.floor-nav-item {
  width: 90rpx;
  height: 90rpx;
  background: #fff;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 24rpx;
  font-weight: 600;
  color: #444;
  box-shadow: 0 2rpx 8rpx rgba(0, 0, 0, 0.12);
  transition: all 0.3s ease;
  border: 1rpx solid rgba(124, 58, 237, 0.1);
}
.floor-nav-item.active {
  background: linear-gradient(135deg, #7c3aed 0%, #a855f7 100%);
  color: #fff;
  box-shadow: 0 4rpx 12rpx rgba(124, 58, 237, 0.3);
  transform: scale(1.05);
}
.floor-nav-item:active {
  transform: scale(0.9);
}

/* 空状态 & 加载更多 */
:deep(.u-empty) {
  margin-top: 200rpx !important;
}
:deep(.u-empty__text) {
  color: #999 !important;
  font-size: 28rpx !important;
}

:deep(.u-loadmore) {
  margin: 24rpx 0 !important;
}
:deep(.u-loadmore__text) {
  color: #999 !important;
  font-size: 26rpx !important;
}
</style>