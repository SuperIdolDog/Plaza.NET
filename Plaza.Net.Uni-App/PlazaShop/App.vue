<script >
	
	export default {
		onLaunch: function() {
			console.log('App Launch');
			  const token = uni.getStorageSync('token');
			  if (token) {
			    // 全局默认 header
			    uni.$u.http.setConfig(config => {
			      config.header.Authorization = `Bearer ${token}`;
			      return config;
			    });
			  }
		},
		onShow: function() {
			console.log('App Show')
		},
		onHide: function() {
			console.log('App Hide')
		}
	}
</script>

<style lang="scss">
		@import "@/uni_modules/uview-ui/index.scss";
	/*每个页面公共css */
	
</style>
