<template>
  <view>
    <!-- 当前商场标题 -->
    <view class="common-title" style="display: flex; align-items: center;">
      <u-icon name="map-fill" size="16" color="red"></u-icon>
      <u-text style="margin-left: 8px;" text="当前城市"></u-text>
    </view>

    <!-- 当前商场信息 -->
    <view class="current-mall" @click="handleCurrentMallClick">
      <u-text class="mall-name" :text=" currentMall.name "></u-text>
      <u-text class="mall-initial" :text="currentMall.initial"></u-text>
    </view>

    <!-- 城市索引列表 -->
    <u-index-list :index-list="indexList" :scroll-top="scrollTop">
      <template v-for="(item, index) in list">
        <u-index-item :key="index">
          <u-index-anchor :text="item.letter" />
          <view class="list-cell u-border-bottom" v-for="(cell, cellIndex) in item.data" :key="cellIndex"
            @click="handleCityClick(cell.name)">
            <u-text class="cell-name" :text="cell.name"></u-text>
          </view>
        </u-index-item>
      </template>
    </u-index-list>
  </view>
</template>

<script>
import indexList from "@/api/city.js";

export default {
  data() {
    return {
      scrollTop: 0,
      indexList: indexList.list.map(item => item.letter),
      list: indexList.list,
      // 当前商场数据（可以从接口或本地存储获取）
      currentMall: {
        name: "", // 默认当前商场
      }
    };
  },
  onLoad(options) {
    if (options.city) {
      this.currentMall.name = decodeURIComponent(options.city);
    }
  },
  methods: {
    handleCityClick(city) {
      console.log("选中城市:", city);
	  uni.$emit('citySelected', city);
	   uni.navigateBack();
    },
    handleCurrentMallClick() {
      console.log("当前商场被点击:", this.currentMall.name);
      uni.showToast({
        title: `当前商场: ${this.currentMall.name}`

      });
    }
  },
  onPageScroll(e) {
    this.scrollTop = e.scrollTop;
  }
};
</script>

<style lang="scss" scoped>
.common-title {
  padding: 12px 24rpx;
  font-size: 14px;
  color: black;
  background-color: #f8f8f8;
}

.current-mall {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 24rpx;
  background-color: #fff;
  border-bottom: 1px solid #f0f0f0;

  .mall-name {
    font-size: 16px;
    color: #333;
  }

  .mall-initial {
    font-size: 14px;
    color: red;
  }
}

.list-cell {
  padding: 12px 24rpx;
  background-color: #fff;

  .cell-name {
    font-size: 16px;
    color: $u-main-color;
  }
}
</style>