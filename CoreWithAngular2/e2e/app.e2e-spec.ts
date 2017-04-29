import { DumDumPage } from './app.po';

describe('dum-dum App', function() {
  let page: DumDumPage;

  beforeEach(() => {
    page = new DumDumPage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
